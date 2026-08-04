import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ACCOUNT_SERVICE, AccountService } from '@services/account.service';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { AccountModel } from '@models/accountModel';

@Component({
  selector: 'app-account-list',
  templateUrl: './account-list.component.html',
  styleUrl: './account-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TranslatePipe],
})
export class AccountListComponent {
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

  open(id: string): void {
    this._router.navigate([this._locale.currentLocale(), 'account', id]);
  }
}
