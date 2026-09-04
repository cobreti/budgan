import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  input,
  signal,
  ViewChild,
} from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { TranslatePipe } from '@ngx-translate/core';
import {
  ACCOUNT_RECURRING_TRANSACTION_SERVICE,
  AccountRecurringTransactionService,
} from '@/services/account-recurring-transaction.service';
import { AccountRecurringTransactionModel } from '@/Models/accountRecurringTransactionModel';
import { AccountModel } from '@/Models/accountModel';

export type AccountRecurringTransactionsViewModel =
  AccountRecurringTransactionModel & {
    accountName: string;
  };

@Component({
  selector: 'app-accounts-recurring-transactions-table',
  templateUrl: './accounts-recurring-transactions-table.component.html',
  styleUrl: './accounts-recurring-transactions-table.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatTableModule, MatSortModule, TranslatePipe],
})
export class AccountRecurringTransactionsTableComponent implements AfterViewInit {
  private readonly _recurringTransactionService =
    inject<AccountRecurringTransactionService>(
      ACCOUNT_RECURRING_TRANSACTION_SERVICE
    );

  @ViewChild(MatSort) sort!: MatSort;

  // readonly accountId = input.required<string>();
  readonly accounts = input.required<AccountModel[]>();

  private readonly _currentPage = signal(0);
  readonly transactions = signal<AccountRecurringTransactionsViewModel[]>([]);
  readonly totalPages = signal(0);
  readonly displayPage = computed(() => this._currentPage() + 1);
  readonly isFirstPage = computed(() => this._currentPage() === 0);
  readonly isLastPage = computed(
    () => this._currentPage() >= this.totalPages() - 1
  );

  readonly pageSize = 25;
  readonly displayedColumns = [
    'accountName',
    'description',
    'firstOccurrenceDate',
    'lastOccurrenceDate',
    'transactionCount',
    'averageAmount',
  ];

  dataSource = new MatTableDataSource<AccountRecurringTransactionsViewModel>();

  constructor() {
    effect(() => {
      this._loadPage(this.accounts());
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  private async _loadPage(accounts: AccountModel[]): Promise<void> {
    const transactions: AccountRecurringTransactionsViewModel[] = [];

    for (const account of accounts) {
      const result = await this._recurringTransactionService.getListByAccount(
        account.id
      );

      transactions.push(
        ...result.map((t) => ({ ...t, accountName: account.name }))
      );
    }

    this.transactions.set(transactions);
    this.dataSource.data = transactions;
  }

  formatAmount(amount: number): string {
    const [intPart, decPart] = amount.toFixed(2).split('.');
    const sign = intPart.startsWith('-') ? '-' : '';
    const intAbs = sign ? intPart.slice(1) : intPart;
    const grouped = intAbs.replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
    return `${sign}${grouped}.${decPart}`;
  }

  isPositiveAmount(row: AccountRecurringTransactionModel): boolean {
    return row.averageAmount > 0;
  }

  isNegativeAmount(row: AccountRecurringTransactionModel): boolean {
    return row.averageAmount < 0;
  }

  getNameForAccountId(accountId: string): string {
    const account = this.accounts().find((a) => a.id === accountId);
    return account ? account.name : accountId;
  }
}
