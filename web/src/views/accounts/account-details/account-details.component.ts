import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatCard, MatCardContent, MatCardTitle } from '@angular/material/card';
import { TranslatePipe } from '@ngx-translate/core';
import { AccountModel } from '@models/accountModel';
import { AccountSnapshotComponent } from '@components/account-snapshot/account-snapshot.component';

@Component({
  selector: 'app-account-details',
  templateUrl: './account-details.component.html',
  styleUrl: './account-details.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [AccountSnapshotComponent, MatCard, MatCardTitle, MatCardContent, TranslatePipe],
})
export class AccountDetailsComponent {
  readonly account = input.required<AccountModel>();

  accountTypeLabelKey(): string {
    return this.account().accountType === 'credit'
      ? 'newAccount.accountTypeCredit'
      : 'newAccount.accountTypeDebit';
  }
}
