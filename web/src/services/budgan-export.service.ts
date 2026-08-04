import { inject, Injectable, InjectionToken } from '@angular/core';
import { COLUMNS_MAPPING_SERVICE, ColumnsMappingService } from './columns-mapping.service';
import { ACCOUNT_SERVICE, AccountService } from './account.service';
import { FILE_SERVICE, FileService } from './file.service';
import { ACCOUNT_TRANSACTION_SERVICE, AccountTransactionService } from './account-transaction.service';
import { ACCOUNT_ANALYSIS_SERVICE, AccountAnalysisService } from './account-analysis.service';
import { ColumnsMapping } from '@models/columnsMappingModel';
import { AccountModel } from '@models/accountModel';
import { fileModel } from '@models/fileModel';
import { AccountTransactionModel } from '@models/accountTransactionModel';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';
import { Result } from '@app-types/result';

export interface AccountExportPayload {
  version: number;
  account: AccountModel;
  columnsMapping: ColumnsMapping;
  files: fileModel[];
  transactions: AccountTransactionModel[];
}

export interface AllDataExportPayload {
  version: number;
  columnsMappings: ColumnsMapping[];
  accounts: AccountModel[];
  files: fileModel[];
  transactions: AccountTransactionModel[];
  recurringTransactions?: AccountRecurringTransactionModel[];
}

export interface BudganExportService {
  pickSaveFile(suggestedName: string): Promise<FileSystemFileHandle | null>;
  pickLoadFile(): Promise<FileSystemFileHandle | null>;
  writeJsonToFile(handle: FileSystemFileHandle, data: unknown): Promise<void>;
  readAllDataPayload(handle: FileSystemFileHandle): Promise<Result<AllDataExportPayload>>;
  parseAllDataPayload(text: string): Result<AllDataExportPayload>;
  buildAccountPayload(accountId: string): Promise<AccountExportPayload>;
  buildAllDataPayload(): Promise<AllDataExportPayload>;
}

export const BUDGAN_EXPORT_SERVICE = new InjectionToken<BudganExportService>('BudganExportService');

@Injectable({ providedIn: 'root' })
export class BudganExportServiceImpl implements BudganExportService {
  private readonly _columnsMappingService = inject<ColumnsMappingService>(COLUMNS_MAPPING_SERVICE);
  private readonly _accountService = inject<AccountService>(ACCOUNT_SERVICE);
  private readonly _fileService = inject<FileService>(FILE_SERVICE);
  private readonly _transactionService = inject<AccountTransactionService>(ACCOUNT_TRANSACTION_SERVICE);
  private readonly _analysisService = inject<AccountAnalysisService>(ACCOUNT_ANALYSIS_SERVICE);

  async pickSaveFile(suggestedName: string): Promise<FileSystemFileHandle | null> {
    const showSaveFilePicker = (window as unknown as {
      showSaveFilePicker: (opts: unknown) => Promise<FileSystemFileHandle>;
    }).showSaveFilePicker;
    try {
      return await showSaveFilePicker({
        suggestedName,
        types: [{ description: 'Budgan files', accept: { 'application/octet-stream': ['.bdg'] } }],
      });
    } catch {
      return null;
    }
  }

  async pickLoadFile(): Promise<FileSystemFileHandle | null> {
    const showOpenFilePicker = (window as unknown as {
      showOpenFilePicker: (opts: unknown) => Promise<FileSystemFileHandle[]>;
    }).showOpenFilePicker;
    try {
      const [handle] = await showOpenFilePicker({
        multiple: false,
        types: [{ description: 'Budgan files', accept: { 'application/octet-stream': ['.bdg'] } }],
      });
      return handle ?? null;
    } catch {
      return null;
    }
  }

  async writeJsonToFile(handle: FileSystemFileHandle, data: unknown): Promise<void> {
    const writable = await handle.createWritable();
    await writable.write(JSON.stringify(data, null, 2));
    await writable.close();
  }

  async readAllDataPayload(handle: FileSystemFileHandle): Promise<Result<AllDataExportPayload>> {
    const text = await (await handle.getFile()).text();
    return this.parseAllDataPayload(text);
  }

  parseAllDataPayload(text: string): Result<AllDataExportPayload> {
    let parsed: unknown;
    try {
      parsed = JSON.parse(text);
    } catch {
      return { success: false, error: 'parse-error' };
    }

    if (!isAllDataExportPayload(parsed)) {
      return { success: false, error: 'invalid-format' };
    }

    return { success: true, value: parsed };
  }

  async buildAccountPayload(accountId: string): Promise<AccountExportPayload> {
    const account = await this._accountService.getById(accountId);
    const [columnsMapping, files, transactions] = await Promise.all([
      this._columnsMappingService.getById(account.columnsMappingId),
      this._fileService.getListByAccount(accountId),
      this._transactionService.getListByAccount(accountId),
    ]);
    return { version: 1, account, columnsMapping, files, transactions };
  }

  async buildAllDataPayload(): Promise<AllDataExportPayload> {
    const [columnsMappings, accounts, files, transactions, recurringTransactions] = await Promise.all([
      this._columnsMappingService.getList(),
      this._accountService.getList(),
      this._fileService.getList(),
      this._transactionService.getList(),
      this._analysisService.getAll(),
    ]);
    return { version: 1, columnsMappings, accounts, files, transactions, recurringTransactions };
  }
}

function isAllDataExportPayload(value: unknown): value is AllDataExportPayload {
  if (typeof value !== 'object' || value === null) return false;
  const v = value as Record<string, unknown>;
  return (
    typeof v['version'] === 'number' &&
    Array.isArray(v['columnsMappings']) &&
    Array.isArray(v['accounts']) &&
    Array.isArray(v['files']) &&
    Array.isArray(v['transactions'])
  );
}
