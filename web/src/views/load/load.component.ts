import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import {
  AllDataExportPayload,
  BUDGAN_EXPORT_SERVICE,
  BudganExportService,
} from '@services/budgan-export.service';
import { isServerBuild } from '@/utils/build-type';
import { IMPORT_SERVICE, ImportService } from '@services/import.service';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '@components/confirm-dialog/confirm-dialog.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';
import { PageComponent } from '@components/page/page.component';

@Component({
  selector: 'app-load',
  templateUrl: './load.component.html',
  styleUrl: './load.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButton, TranslatePipe, PageComponent, PageBodyComponent, PageMenuComponent, PageMenuButtonComponent],
})
export class LoadComponent {
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _exportService = inject<BudganExportService>(BUDGAN_EXPORT_SERVICE);
  private readonly _importService = inject<ImportService>(IMPORT_SERVICE);
  private readonly _dialog = inject(MatDialog);

  readonly selectedFileName = signal<string>('');
  readonly errorKey = signal<string | null>(null);
  readonly summaryKey = signal<string | null>(null);
  readonly summaryParams = signal<Record<string, number>>({});

  private _fileHandle: FileSystemFileHandle | null = null;

  async onSelectFile(): Promise<void> {
    const handle = await this._exportService.pickLoadFile();
    if (!handle) return;

    this._fileHandle = handle;
    this.selectedFileName.set(handle.name);
    this.errorKey.set(null);

    const result = await this._exportService.readAllDataPayload(handle);
    if (!result.success) {
      this.errorKey.set(
        result.error === 'parse-error' ? 'load.parseError' : 'load.invalidFormat',
      );
      return;
    }

    await this.onLoad(result.value);
  }

  async onLoad(payload: AllDataExportPayload): Promise<void> {
    if (isServerBuild()) {
      await this._runImport(payload);
      return;
    }

    const ref = this._dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(
      ConfirmDialogComponent,
      {
        data: {
          title: 'load.confirmTitle',
          message: 'load.confirmMessage',
          confirmLabel: 'load.confirmAccept',
          cancelLabel: 'load.confirmCancel',
          danger: true,
        },
      },
    );

    const confirmed = await ref.afterClosed().toPromise();
    if (!confirmed) return;

    await this._runImport(payload);
  }

  private async _runImport(payload: AllDataExportPayload): Promise<void> {
    this.summaryKey.set(null);
    const summary = await this._importService.importAllData(payload);

    if (summary.skipped === 0 && summary.errors === 0) {
      await this._router.navigate([this._locale.currentLocale()]);
      return;
    }

    this.summaryParams.set({ skipped: summary.skipped, errors: summary.errors });
    this.summaryKey.set('load.completedWithIssues');
  }

  onCancel(): void {
    this._router.navigate([this._locale.currentLocale()]);
  }
}
