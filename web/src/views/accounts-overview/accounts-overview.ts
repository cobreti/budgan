import { PageBodyComponent } from '@/components/page-body/page-body.component';
import { PageComponent } from '@/components/page/page.component';
import { AccountModel } from '@/Models/accountModel';
import { ACCOUNT_SERVICE, AccountService } from '@/services/account.service';
import { Component, inject, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { AccountRecurringTransactionsTableComponent } from './accounts-recurring-transactions-table/accounts-recurring-transactions-table.component';
import { PageMenuComponent } from '@/components/page-menu/page-menu.component';
import { Router } from '@angular/router';
import { LOCALE_SERVICE, LocaleService } from '@/services/locale.service';
import { PageMenuButtonComponent } from '@/components/page-menu/page-menu-button/page-menu-button.component';
import { MatTab, MatTabGroup } from '@angular/material/tabs';

@Component({
  selector: 'app-accounts-overview',
  templateUrl: './accounts-overview.html',
  styleUrl: './accounts-overview.scss',
  imports: [
    TranslatePipe,
    PageComponent,
    PageMenuComponent,
    PageMenuButtonComponent,
    PageBodyComponent,
    AccountRecurringTransactionsTableComponent,
    MatTabGroup,
    MatTab,
  ],
})
export class AccountOverviewComponent {
  private readonly _accountService = inject<AccountService>(ACCOUNT_SERVICE);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);

  readonly accounts = signal<AccountModel[]>([]);

  constructor() {
    this.refresh();
  }

  async refresh(): Promise<void> {
    this.accounts.set(await this._accountService.getList());
  }

  onBack(): void {
    this._router.navigate([this._locale.currentLocale()]);
  }
}
