import { inject, Injectable } from '@angular/core';
import { AllDataExportPayload } from '@services/budgan-export.service';
import {
  COLUMNS_MAPPING_SERVICE,
  ColumnsMappingService,
} from '@services/columns-mapping.service';
import { ACCOUNT_SERVICE, AccountService } from '@services/account.service';
import { FILE_SERVICE, FileService } from '@services/file.service';
import {
  ACCOUNT_TRANSACTION_SERVICE,
  AccountTransactionService,
} from '@services/account-transaction.service';
import {
  ACCOUNT_ANALYSIS_SERVICE,
  AccountAnalysisService,
} from '@services/account-analysis.service';
import { ImportService, ImportSummary } from '@services/import.service';
import { runImport } from '@/services/service-utils/import-runner';

@Injectable({ providedIn: 'root' })
export class ImportServiceServerImpl implements ImportService {
  private readonly _columnsMappingService = inject<ColumnsMappingService>(
    COLUMNS_MAPPING_SERVICE,
  );
  private readonly _accountService = inject<AccountService>(ACCOUNT_SERVICE);
  private readonly _fileService = inject<FileService>(FILE_SERVICE);
  private readonly _transactionService = inject<AccountTransactionService>(
    ACCOUNT_TRANSACTION_SERVICE,
  );
  private readonly _analysisService = inject<AccountAnalysisService>(
    ACCOUNT_ANALYSIS_SERVICE,
  );

  async importAllData(payload: AllDataExportPayload): Promise<ImportSummary> {
    return runImport(
      {
        columnsMappingService: this._columnsMappingService,
        accountService: this._accountService,
        fileService: this._fileService,
        transactionService: this._transactionService,
        analysisService: this._analysisService,
      },
      payload,
    );
  }
}
