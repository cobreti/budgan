import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { AUTH_SERVICE } from '@services/auth.service';
import { isServerBuild } from '@/utils/build-type';
import { Role } from '@/types/role';
import {
  DuplicateTransaction,
  IMPORT_CSV_TRANSACTIONS_SERVICE,
  ImportCsvTransactionsService,
} from '@services/import-csv-transactions.service';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { PageComponent } from '@components/page/page.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';
import {
  DemoStatementPickerComponent,
  DemoStatementPickerData,
} from '@components/demo-statement-picker/demo-statement-picker.component';

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
  imports: [MatButton, MatIcon, NgTemplateOutlet, TranslatePipe, PageComponent, PageBodyComponent, PageMenuComponent, PageMenuButtonComponent],
})
export class ImportFileComponent {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _importCsvTransactionsService = inject<ImportCsvTransactionsService>(IMPORT_CSV_TRANSACTIONS_SERVICE);
  private readonly _dialog = inject(MatDialog);
  private readonly _authService = inject(AUTH_SERVICE);

  private readonly _accountId = this._route.snapshot.params['accountId'] as string;

  readonly selectedFiles = signal<File[]>([]);
  readonly importStatuses = signal<FileImportStatus[]>([]);
  readonly filesSource = signal<'native' | 'demo' | null>(null);
  readonly isImporting = signal<boolean>(false);
  readonly importCompleted = signal<boolean>(false);
  readonly hasSelectedFiles = computed(() => this.selectedFiles().length > 0);
  readonly canChooseCsvFile = computed(
    () => !isServerBuild() || this._authService.roles().includes(Role.PersonalDataAllowed),
  );

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);
    if (files.length === 0) return;
    this.setSelectedFiles(files, 'native');
    input.value = '';
  }

  async onLoadDemoStatements(): Promise<void> {
    const ref = this._dialog.open<DemoStatementPickerComponent, DemoStatementPickerData, File[]>(
      DemoStatementPickerComponent,
      { data: { multiple: true } },
    );
    const files = await ref.afterClosed().toPromise();
    if (!files || files.length === 0) return;
    this.setSelectedFiles(files, 'demo');
  }

  private setSelectedFiles(files: File[], source: 'native' | 'demo'): void {
    this.selectedFiles.set(files);
    this.importStatuses.set(files.map(file => ({ file, status: 'pending' })));
    this.filesSource.set(source);
    this.importCompleted.set(false);
  }

  async onImport(): Promise<void> {
    if (!this.hasSelectedFiles()) return;
    this.isImporting.set(true);

    const summary = await this._importCsvTransactionsService.importFiles(
      this._accountId,
      this.selectedFiles(),
      i => this.importStatuses.update(s => s.map((x, idx) => idx === i ? { ...x, status: 'importing' } : x)),
      (i, outcome) => this.importStatuses.update(s => s.map((x, idx) => {
        if (idx !== i) return x;
        if (outcome.status === 'success') return { ...x, status: 'success' };
        if (outcome.status === 'warning') return { ...x, status: 'warning', duplicates: outcome.duplicates };
        return {
          ...x, status: 'error',
          error: outcome.error === 'file-already-imported'
            ? 'importFile.fileAlreadyImported'
            : 'importFile.csvParseError',
        };
      })),
    );

    this.isImporting.set(false);
    this.importCompleted.set(true);

    if (!summary.anyError && !summary.anyWarning) {
      await this._router.navigate([this._locale.currentLocale(), 'account', this._accountId]);
    }
  }

  onCancel(): void {
    this._router.navigate([this._locale.currentLocale(), 'account', this._accountId]);
  }
}
