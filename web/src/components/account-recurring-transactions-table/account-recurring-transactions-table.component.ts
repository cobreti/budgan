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

@Component({
  selector: 'app-account-recurring-transactions-table',
  templateUrl: './account-recurring-transactions-table.component.html',
  styleUrl: './account-recurring-transactions-table.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatTableModule, MatSortModule, TranslatePipe],
})
export class AccountRecurringTransactionsTableComponent implements AfterViewInit {
  private readonly _recurringTransactionService =
    inject<AccountRecurringTransactionService>(
      ACCOUNT_RECURRING_TRANSACTION_SERVICE,
    );

  @ViewChild(MatSort) sort!: MatSort;

  readonly accountId = input.required<string>();

  private readonly _currentPage = signal(0);
  readonly transactions = signal<AccountRecurringTransactionModel[]>([]);
  readonly totalPages = signal(0);
  readonly displayPage = computed(() => this._currentPage() + 1);
  readonly isFirstPage = computed(() => this._currentPage() === 0);
  readonly isLastPage = computed(
    () => this._currentPage() >= this.totalPages() - 1,
  );

  readonly pageSize = 25;
  readonly displayedColumns = [
    'description',
    'firstOccurrenceDate',
    'lastOccurrenceDate',
    'transactionCount',
    'averageAmount',
  ];

  dataSource = new MatTableDataSource<AccountRecurringTransactionModel>();

  constructor() {
    effect(() => {
      const accountId = this.accountId();
      this._loadPage(accountId);
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  private async _loadPage(accountId: string): Promise<void> {
    const result =
      await this._recurringTransactionService.getListByAccount(accountId);
    this.transactions.set(result);
    this.dataSource.data = result;
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
}
