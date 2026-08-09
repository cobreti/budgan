import { AllDataExportPayload } from '@services/budgan-export.service';
import { ColumnsMappingService } from '@services/columns-mapping.service';
import { AccountService } from '@services/account.service';
import { FileService } from '@services/file.service';
import { AccountTransactionService } from '@services/account-transaction.service';
import { AccountAnalysisService } from '@services/account-analysis.service';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';
import { fileModel } from '@models/fileModel';
import { AccountTransactionRecordType } from '@models/accountTransactionModel';
import { ImportSummary } from '@services/import.service';

export interface ImportRunnerDeps {
  columnsMappingService: ColumnsMappingService;
  accountService: AccountService;
  fileService: FileService;
  transactionService: AccountTransactionService;
  analysisService: AccountAnalysisService;
}

export async function runImport(
  deps: ImportRunnerDeps,
  payload: AllDataExportPayload,
): Promise<ImportSummary> {
  let skipped = 0;
  let errors = 0;

  const mappingOk = new Set<string>();
  for (const mapping of payload.columnsMappings) {
    const result = await deps.columnsMappingService.save(mapping);
    if (result.success) {
      mappingOk.add(mapping.id!);
    } else if (result.error === 'id-exists') {
      mappingOk.add(mapping.id!);
      skipped++;
    } else {
      errors++;
    }
  }

  const accountOk = new Set<string>();
  for (const account of payload.accounts) {
    if (!mappingOk.has(account.columnsMappingId)) {
      errors++;
      continue;
    }

    const result = await deps.accountService.create(
      account.name,
      account.columnsMappingId,
      account.accountType ?? 'debit',
      account.id,
    );
    if (result.success) {
      accountOk.add(account.id);
    } else if (result.error === 'id-exists') {
      accountOk.add(account.id);
      skipped++;
    } else {
      errors++;
    }
  }

  const fileIdMap = new Map<string, string>();
  const filesByAccount = new Map<string, fileModel[]>();
  for (const file of payload.files) {
    if (!accountOk.has(file.accountId)) {
      errors++;
      continue;
    }

    const result = await deps.fileService.create(
      file.accountId,
      file.filename,
      file.content,
      new Date(file.insertionDate),
    );
    if (result.success) {
      fileIdMap.set(file.id, result.value);
    } else if (result.error === 'file-already-imported') {
      let existing = filesByAccount.get(file.accountId);
      if (!existing) {
        existing = await deps.fileService.getListByAccount(file.accountId);
        filesByAccount.set(file.accountId, existing);
      }
      const match = existing.find((f) => f.filename === file.filename);
      if (match) {
        fileIdMap.set(file.id, match.id);
        skipped++;
      } else {
        errors++;
      }
    } else {
      errors++;
    }
  }

  const touchedAccounts = new Set<string>();
  for (const tx of payload.transactions) {
    if (!accountOk.has(tx.accountId)) {
      errors++;
      continue;
    }

    if (tx.recordType === AccountTransactionRecordType.snapshot) {
      const result = await deps.transactionService.setSnapshot(
        tx.accountId,
        tx.dateInscriptionAsString,
        tx.amount,
      );
      if (result.success) {
        touchedAccounts.add(tx.accountId);
      } else {
        errors++;
      }
      continue;
    }

    const mappedFileId = fileIdMap.get(tx.fileId);
    if (!mappedFileId) {
      errors++;
      continue;
    }

    const result = await deps.transactionService.create(
      mappedFileId,
      tx.accountId,
      tx.cardNumber,
      tx.dateInscriptionAsString,
      tx.amount,
      tx.description,
    );
    if (result.success) {
      touchedAccounts.add(tx.accountId);
    } else if (result.error === 'duplicate-transaction') {
      skipped++;
    } else {
      errors++;
    }
  }

  for (const accountId of touchedAccounts) {
    await deps.transactionService.recalculateBalances(accountId);
  }

  if (payload.recurringTransactions?.length) {
    const byAccount = new Map<string, AccountRecurringTransactionModel[]>();
    for (const rt of payload.recurringTransactions) {
      if (!accountOk.has(rt.accountId)) {
        errors++;
        continue;
      }
      const list = byAccount.get(rt.accountId) ?? [];
      list.push(rt);
      byAccount.set(rt.accountId, list);
    }
    for (const [accountId, transactions] of byAccount) {
      const result = await deps.analysisService.applyAnalysis(accountId, transactions);
      if (!result.success) errors++;
    }
  }

  return { skipped, errors };
}
