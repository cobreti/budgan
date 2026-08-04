import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { BUDGAN_EXPORT_SERVICE, BudganExportService } from '@services/budgan-export.service';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';
import { PageComponent } from '@components/page/page.component';

@Component({
  selector: 'app-save-account',
  templateUrl: './save-account.component.html',
  styleUrl: './save-account.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButton, TranslatePipe, PageComponent, PageBodyComponent, PageMenuComponent, PageMenuButtonComponent],
})
export class SaveAccountComponent {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _exportService = inject<BudganExportService>(BUDGAN_EXPORT_SERVICE);

  private readonly _accountId = this._route.snapshot.params['accountId'] as string;

  readonly selectedFileName = signal<string>('');

  private _fileHandle: FileSystemFileHandle | null = null;

  async onSelectFile(): Promise<void> {
    const handle = await this._exportService.pickSaveFile('account.bdg');
    if (handle) {
      this._fileHandle = handle;
      this.selectedFileName.set(handle.name);

      await this.onSave();
    }
  }

  async onSave(): Promise<void> {
    if (!this._fileHandle) return;

    const payload = await this._exportService.buildAccountPayload(this._accountId);
    await this._exportService.writeJsonToFile(this._fileHandle, payload);

    await this._router.navigate([this._locale.currentLocale(), 'account', this._accountId]);
  }

  onCancel(): void {
    this._router.navigate([this._locale.currentLocale(), 'account', this._accountId]);
  }
}
