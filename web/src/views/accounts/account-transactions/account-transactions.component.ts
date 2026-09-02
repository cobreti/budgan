import {
  ChangeDetectionStrategy,
  Component,
  input,
  signal,
} from '@angular/core';
import { AccountTransactionsTableComponent } from '@components/account-transactions-table/account-transactions-table.component';
import {
  MatButtonToggle,
  MatButtonToggleGroup,
} from '@angular/material/button-toggle';
import { TranslatePipe } from '@ngx-translate/core';
import { AccountRecurringTransactionsTableComponent } from '@/components/account-recurring-transactions-table/account-recurring-transactions-table.component';

export type TransactionsDisplay = 'all' | 'recurring';

@Component({
  selector: 'app-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrl: './account-transactions.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    AccountTransactionsTableComponent,
    AccountRecurringTransactionsTableComponent,
    MatButtonToggle,
    MatButtonToggleGroup,
    TranslatePipe,
  ],
})
export class AccountTransactionsComponent {
  readonly accountId = input.required<string>();

  transactionsDisplay = signal<TransactionsDisplay>('all');
}
