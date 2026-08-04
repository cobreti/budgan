import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { FILE_SERVICE, FileService } from '@services/file.service';
import { ACCOUNT_SERVICE, AccountService } from '@services/account.service';
import { COLUMNS_MAPPING_SERVICE, ColumnsMappingService } from '@services/columns-mapping.service';
import { CSV_CONTENT_EXTRACTOR_SERVICE, CsvContentExtractorService } from '@services/csv-content-extractor.service';
import { ACCOUNT_TRANSACTION_SERVICE, AccountTransactionService } from '@services/account-transaction.service';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { PageComponent } from '@components/page/page.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';
import { formatIsoDate, parseIsoDate, parseYYYYMMDD } from '@/utils/date';

interface DuplicateTransaction {
  cardNumber: string;
  dateInscriptionAsString: string;
  amount: number;
  description: string;
}

interface FileImportStatus {
  file: File;
  status: 'pending' | 'importing' | 'success' | 'warning' | 'error';
  error?: string;
  duplicates?: DuplicateTransaction[];
}

@Component({
  selector: 'app-import-file',
  templateUrl: './import-file.component.html',
  styleUrl: './import-file.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButton, MatIcon, TranslatePipe, PageComponent, PageBodyComponent, PageMenuComponent, PageMenuButtonComponent],
})
export class ImportFileComponent {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _fileService = inject<FileService>(FILE_SERVICE);
  private readonly _accountService = inject<AccountService>(ACCOUNT_SERVICE);
  private readonly _columnsMappingService = inject<ColumnsMappingService>(COLUMNS_MAPPING_SERVICE);
  private readonly _csvExtractor = inject<CsvContentExtractorService>(CSV_CONTENT_EXTRACTOR_SERVICE);
  private readonly _transactionService = inject<AccountTransactionService>(ACCOUNT_TRANSACTION_SERVICE);

  private readonly _accountId = this._route.snapshot.params['accountId'] as string;

  readonly selectedFiles = signal<File[]>([]);
  readonly importStatuses = signal<FileImportStatus[]>([]);
  readonly isImporting = signal<boolean>(false);
  readonly importCompleted = signal<boolean>(false);
  readonly hasSelectedFiles = computed(() => this.selectedFiles().length > 0);

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);
    if (files.length === 0) return;
    this.selectedFiles.set(files);
    this.importStatuses.set(files.map(file => ({ file, status: 'pending' })));
    this.importCompleted.set(false);
    input.value = '';
  }

  async onImport(): Promise<void> {
    if (!this.hasSelectedFiles()) return;
    this.isImporting.set(true);

    const account = await this._accountService.getById(this._accountId);
    const mapping = await this._columnsMappingService.getById(account.columnsMappingId);

    let anySuccess = false;
    let anyError = false;
    let anyWarning = false;

    for (let i = 0; i < this.selectedFiles().length; i++) {
      const file = this.selectedFiles()[i];
      this.importStatuses.update(s => s.map((x, idx) => idx === i ? { ...x, status: 'importing' } : x));

      const content = await file.text();
      const extractionResult = this._csvExtractor.extract(content);

      if (!extractionResult.success) {
        anyError = true;
        this.importStatuses.update(s => s.map((x, idx) =>
          idx === i ? { ...x, status: 'error', error: 'importFile.csvParseError' } : x));
        continue;
      }

      const { header, rows } = extractionResult.value;
      const fileResult = await this._fileService.create(this._accountId, file.name, content, new Date());

      if (!fileResult.success) {
        anyError = true;
        this.importStatuses.update(s => s.map((x, idx) =>
          idx === i ? {
            ...x, status: 'error',
            error: fileResult.error === 'file-already-imported'
              ? 'importFile.fileAlreadyImported'
              : 'importFile.csvParseError',
          } : x));
        continue;
      }

      const fileId = fileResult.value;
      const duplicates: DuplicateTransaction[] = [];

      for (const row of rows) {
        const cardNumber = row[header[mapping.cardNumberColumnIndex]] ?? '';
        const dateInscription = parseYYYYMMDD(row[header[mapping.dateInscriptionColumnIndex]]);
        const dateInscriptionAsString = formatIsoDate(dateInscription);
        const amountStr = (row[header[mapping.amountColumnIndex]] ?? '').replace(',', '.');
        const parsedAmount = parseFloat(amountStr);
        if (isNaN(parsedAmount)) continue;
        const amount = account.accountType === 'credit' ? -parsedAmount : parsedAmount;
        const description = row[header[mapping.descriptionColumnIndex]] ?? '';
        const result = await this._transactionService.create(fileId, this._accountId, cardNumber, dateInscriptionAsString, amount, description);
        if (!result.success && result.error === 'duplicate-transaction') {
          duplicates.push({ cardNumber, dateInscriptionAsString, amount, description });
        }
      }

      if (duplicates.length > 0) {
        anyWarning = true;
        this.importStatuses.update(s => s.map((x, idx) =>
          idx === i ? { ...x, status: 'warning', duplicates } : x));
      } else {
        anySuccess = true;
        this.importStatuses.update(s => s.map((x, idx) => idx === i ? { ...x, status: 'success' } : x));
      }
    }

    if (anySuccess || anyWarning) {
      await this._transactionService.recalculateBalances(this._accountId);
    }

    this.isImporting.set(false);
    this.importCompleted.set(true);

    if (!anyError && !anyWarning) {
      await this._router.navigate([this._locale.currentLocale(), 'account', this._accountId]);
    }
  }

  onCancel(): void {
    this._router.navigate([this._locale.currentLocale(), 'account', this._accountId]);
  }
}
