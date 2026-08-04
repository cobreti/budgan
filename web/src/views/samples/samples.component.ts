import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { MatButton } from '@angular/material/button';
import { MatCard, MatCardContent, MatCardTitle } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { BUDGAN_EXPORT_SERVICE, BudganExportService } from '@services/budgan-export.service';
import { IndexdbService } from '@services/indexdb.service';
import { PageComponent } from '@components/page/page.component';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '@components/confirm-dialog/confirm-dialog.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';

@Component({
  selector: 'app-samples',
  templateUrl: './samples.component.html',
  styleUrl: './samples.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    TranslatePipe,
    PageComponent,
    PageBodyComponent,
    PageMenuComponent,
    PageMenuButtonComponent,
    MatCard,
    MatCardTitle,
    MatCardContent,
    MatButton,
  ],
})
export class SamplesComponent {
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _http = inject(HttpClient);
  private readonly _exportService = inject<BudganExportService>(BUDGAN_EXPORT_SERVICE);
  private readonly _indexdb = inject(IndexdbService);
  private readonly _dialog = inject(MatDialog);

  readonly errorKey = signal<string | null>(null);

  async onLoadSample(): Promise<void> {
    this.errorKey.set(null);

    let text: string;
    try {
      text = await firstValueFrom(
        this._http.get('/assets/samples/sample.json', { responseType: 'text' }),
      );
    } catch {
      this.errorKey.set('samples.loadError');
      return;
    }

    const result = this._exportService.parseAllDataPayload(text);
    if (!result.success) {
      this.errorKey.set('samples.loadError');
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

    await this._indexdb.replaceAll(result.value);
    await this._router.navigate([this._locale.currentLocale()]);
  }

  async onBack(): Promise<void> {
    await this._router.navigate([this._locale.currentLocale()]);
  }
}
