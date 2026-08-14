import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';
import { ACCOUNT_SERVICE, AccountService } from '@services/account.service';
import { COLUMNS_MAPPING_SERVICE, ColumnsMappingService } from '@services/columns-mapping.service';
import { FILE_SERVICE, FileService } from '@services/file.service';
import {
  CSV_CONTENT_EXTRACTOR_SERVICE,
  CsvContentExtractorService,
} from '@services/csv-content-extractor.service';
import {
  ACCOUNT_TRANSACTION_SERVICE,
  AccountTransactionService,
} from '@services/account-transaction.service';
import { ImportCsvTransactionsServiceImpl } from './import-csv-transactions.service';
import { AccountModel } from '@models/accountModel';
import { ColumnsMapping } from '@models/columnsMappingModel';

const account: AccountModel = {
  id: 'account-1',
  name: 'Test Account',
  columnsMappingId: 'mapping-1',
  accountType: 'debit',
};

function buildMapping(dateFormat: string): ColumnsMapping {
  return {
    id: 'mapping-1',
    name: 'Test Mapping',
    cardNumberColumnIndex: 0,
    dateInscriptionColumnIndex: 1,
    amountColumnIndex: 2,
    descriptionColumnIndex: 3,
    dateFormat,
  };
}

describe('ImportCsvTransactionsServiceImpl', () => {
  let accountService: AccountService;
  let columnsMappingService: ColumnsMappingService;
  let fileService: FileService;
  let csvExtractor: CsvContentExtractorService;
  let transactionService: AccountTransactionService;
  let service: ImportCsvTransactionsServiceImpl;

  beforeEach(() => {
    accountService = { getById: vi.fn().mockResolvedValue(account) } as unknown as AccountService;
    fileService = {
      create: vi.fn().mockResolvedValue({ success: true, value: 'file-1' }),
    } as unknown as FileService;
    transactionService = {
      create: vi.fn().mockResolvedValue({ success: true, value: 'tx-1' }),
      recalculateBalances: vi.fn().mockResolvedValue(undefined),
    } as unknown as AccountTransactionService;

    TestBed.configureTestingModule({
      providers: [
        { provide: ACCOUNT_SERVICE, useValue: accountService },
        { provide: COLUMNS_MAPPING_SERVICE, useFactory: () => columnsMappingService },
        { provide: FILE_SERVICE, useValue: fileService },
        { provide: CSV_CONTENT_EXTRACTOR_SERVICE, useFactory: () => csvExtractor },
        { provide: ACCOUNT_TRANSACTION_SERVICE, useValue: transactionService },
      ],
    });
  });

  it('uses the configured dateFormat regex to parse dates', async () => {
    columnsMappingService = {
      getById: vi.fn().mockResolvedValue(buildMapping('(?<day>\\d{2})/(?<month>\\d{2})/(?<year>\\d{4})')),
    } as unknown as ColumnsMappingService;
    csvExtractor = {
      extract: vi.fn().mockReturnValue({
        success: true,
        value: {
          delimiter: ',',
          headerRowIndex: 0,
          header: ['card', 'date', 'amount', 'description'],
          rows: [{ card: '1234', date: '15/03/2024', amount: '10.00', description: 'Coffee' }],
        },
      }),
    } as unknown as CsvContentExtractorService;
    service = TestBed.inject(ImportCsvTransactionsServiceImpl);

    const file = new File(['card,date,amount,description'], 'test.csv', { type: 'text/csv' });
    const onFileStart = vi.fn();
    const onFileDone = vi.fn();

    const summary = await service.importFiles('account-1', [file], onFileStart, onFileDone);

    expect(summary.anyError).toBe(false);
    expect(summary.anySuccess).toBe(true);
    expect(transactionService.create).toHaveBeenCalledWith(
      'file-1',
      'account-1',
      '1234',
      '2024-03-15',
      10,
      'Coffee',
    );
  });

  it('skips a row whose date does not match the configured format and reports an error', async () => {
    columnsMappingService = {
      getById: vi.fn().mockResolvedValue(buildMapping('(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})')),
    } as unknown as ColumnsMappingService;
    csvExtractor = {
      extract: vi.fn().mockReturnValue({
        success: true,
        value: {
          delimiter: ',',
          headerRowIndex: 0,
          header: ['card', 'date', 'amount', 'description'],
          rows: [{ card: '1234', date: 'not-a-date', amount: '10.00', description: 'Coffee' }],
        },
      }),
    } as unknown as CsvContentExtractorService;
    service = TestBed.inject(ImportCsvTransactionsServiceImpl);

    const file = new File(['card,date,amount,description'], 'test.csv', { type: 'text/csv' });

    const summary = await service.importFiles('account-1', [file], vi.fn(), vi.fn());

    expect(summary.anyError).toBe(true);
    expect(transactionService.create).not.toHaveBeenCalled();
  });
});
