import { Component, computed, inject, viewChild } from '@angular/core';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { TranslatePipe } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { PageComponent } from '@components/page/page.component';
import { MatCard } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { ColumnsMappingListComponent } from '@components/columns-mapping-list/columns-mapping-list.component';
import { AccountListComponent } from '@components/account-list/account-list.component';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '@components/confirm-dialog/confirm-dialog.component';
import { IndexdbService } from '@services/indexdb.service';
import { PageBodyComponent } from '@components/page-body/page-body.component';

@Component({
  templateUrl: 'home.component.html',
  styleUrls: ['home.component.scss'],
  selector: 'app-home',
  imports: [
    PageBodyComponent,
    PageMenuComponent,
    PageMenuButtonComponent,
    TranslatePipe,
    PageComponent,
    ColumnsMappingListComponent,
    AccountListComponent,
    MatCard,
  ],
})
export class HomeComponent {
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _dialog = inject(MatDialog);
  private readonly _indexdb = inject(IndexdbService);

  private readonly _columnsMappingList = viewChild.required(ColumnsMappingListComponent);
  private readonly _accountList = viewChild.required(AccountListComponent);

  readonly hasData = computed(
    () =>
      this._accountList().accounts().length > 0 ||
      this._columnsMappingList().mappings().length > 0,
  );

  async onNewColumnsMapping(): Promise<void> {
    await this._router.navigate([this._locale.currentLocale(), 'columns-mapping', 'new']);
  }

  async onNewAccount(): Promise<void> {
    await this._router.navigate([this._locale.currentLocale(), 'account', 'new']);
  }

  async onSave(): Promise<void> {
    await this._router.navigate([this._locale.currentLocale(), 'save']);
  }

  async onLoad(): Promise<void> {
    await this._router.navigate([this._locale.currentLocale(), 'load']);
  }

  async onSamples(): Promise<void> {
    await this._router.navigate([this._locale.currentLocale(), 'samples']);
  }

  async onClearAll(): Promise<void> {
    const ref = this._dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(
      ConfirmDialogComponent,
      {
        data: {
          title: 'clearAll.title',
          message: 'clearAll.message',
          confirmLabel: 'clearAll.confirm',
          cancelLabel: 'clearAll.cancel',
          danger: true,
        },
      },
    );

    const confirmed = await ref.afterClosed().toPromise();
    if (!confirmed) {
      return;
    }

    await this._indexdb.clearAll();
    await Promise.all([
      this._columnsMappingList().refresh(),
      this._accountList().refresh(),
    ]);
  }
}
