import { inject, Injectable, InjectionToken } from '@angular/core';
import { formatIsoDate, parseDateWithFormat } from '@/utils/date';
import { ACCOUNT_SERVICE, AccountService } from '@services/account.service';
import { COLUMNS_MAPPING_SERVICE, ColumnsMappingService } from '@services/columns-mapping.service';
import { FILE_SERVICE, FileService } from '@services/file.service';
import { CSV_CONTENT_EXTRACTOR_SERVICE, CsvContentExtractorService } from '@services/csv-content-extractor.service';
import { ACCOUNT_TRANSACTION_SERVICE, AccountTransactionService } from '@services/account-transaction.service';

export interface DuplicateTransaction {
  cardNumber: string;
  dateInscriptionAsString: string;
  amount: number;
  description: string;
}

export type FileImportOutcome =
  | { status: 'success' }
  | { status: 'warning'; duplicates: DuplicateTransaction[] }
  | { status: 'error'; error: string };

export type ImportFilesSummary = {
  anySuccess: boolean;
  anyError: boolean;
  anyWarning: boolean;
};

export interface ImportCsvTransactionsService {
  importFiles(
    accountId: string,
    files: File[],
    onFileStart: (index: number) => void,
    onFileDone: (index: number, outcome: FileImportOutcome) => void,
  ): Promise<ImportFilesSummary>;
}

export const IMPORT_CSV_TRANSACTIONS_SERVICE = new InjectionToken<ImportCsvTransactionsService>(
  'ImportCsvTransactionsService',
);

@Injectable({ providedIn: 'root' })
export class ImportCsvTransactionsServiceImpl implements ImportCsvTransactionsService {
  private readonly _accountService = inject<AccountService>(ACCOUNT_SERVICE);
  private readonly _columnsMappingService = inject<ColumnsMappingService>(COLUMNS_MAPPING_SERVICE);
  private readonly _fileService = inject<FileService>(FILE_SERVICE);
  private readonly _csvExtractor = inject<CsvContentExtractorService>(CSV_CONTENT_EXTRACTOR_SERVICE);
  private readonly _transactionService = inject<AccountTransactionService>(ACCOUNT_TRANSACTION_SERVICE);

  async importFiles(
    accountId: string,
    files: File[],
    onFileStart: (index: number) => void,
    onFileDone: (index: number, outcome: FileImportOutcome) => void,
  ): Promise<ImportFilesSummary> {
    const account = await this._accountService.getById(accountId);
    const mapping = await this._columnsMappingService.getById(account.columnsMappingId);

    let anySuccess = false;
    let anyError = false;
    let anyWarning = false;

    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      onFileStart(i);

      const content = await file.text();
      const extractionResult = this._csvExtractor.extract(content);

      if (!extractionResult.success) {
        anyError = true;
        onFileDone(i, { status: 'error', error: 'csv-parse-error' });
        continue;
      }

      const { header, rows } = extractionResult.value;
      const fileResult = await this._fileService.create(accountId, file.name, content, new Date());

      if (!fileResult.success) {
        anyError = true;
        onFileDone(i, { status: 'error', error: fileResult.error });
        continue;
      }

      const fileId = fileResult.value;
      const duplicates: DuplicateTransaction[] = [];

      for (const row of rows) {
        const cardNumber = row[header[mapping.cardNumberColumnIndex]] ?? '';
        const rawDateValue = row[header[mapping.dateInscriptionColumnIndex]];
        const parsedDate = parseDateWithFormat(rawDateValue, mapping.dateFormat);
        if (!parsedDate) {
          anyError = true;
          continue;
        }
        const dateInscriptionAsString = formatIsoDate(parsedDate);
        const amountStr = (row[header[mapping.amountColumnIndex]] ?? '').replace(',', '.');
        const parsedAmount = parseFloat(amountStr);
        if (isNaN(parsedAmount)) continue;
        const amount = account.accountType === 'credit' ? -parsedAmount : parsedAmount;
        const description = row[header[mapping.descriptionColumnIndex]] ?? '';
        const result = await this._transactionService.create(fileId, accountId, cardNumber, dateInscriptionAsString, amount, description);
        if (!result.success && result.error === 'duplicate-transaction') {
          duplicates.push({ cardNumber, dateInscriptionAsString, amount, description });
        }
      }

      if (duplicates.length > 0) {
        anyWarning = true;
        onFileDone(i, { status: 'warning', duplicates });
      } else {
        anySuccess = true;
        onFileDone(i, { status: 'success' });
      }
    }

    if (anySuccess || anyWarning) {
      await this._transactionService.recalculateBalances(accountId);
    }

    return { anySuccess, anyError, anyWarning };
  }
}
