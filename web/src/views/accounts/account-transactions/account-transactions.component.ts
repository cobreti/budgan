import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { AccountTransactionsTableComponent } from '@components/account-transactions-table/account-transactions-table.component';

@Component({
  selector: 'app-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrl: './account-transactions.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [AccountTransactionsTableComponent],
})
export class AccountTransactionsComponent {
  readonly accountId = input.required<string>();
}
