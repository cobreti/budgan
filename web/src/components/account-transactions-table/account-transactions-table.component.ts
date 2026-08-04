import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import {
  ACCOUNT_TRANSACTION_SERVICE,
  AccountTransactionService,
  TransactionSort,
  TransactionSortField,
} from '@services/account-transaction.service';
import { AccountTransactionModel, AccountTransactionRecordType } from '@models/accountTransactionModel';

const DEFAULT_SORT: TransactionSort = { field: 'dateInscription', direction: 'desc' };
const SORTABLE_FIELDS: readonly TransactionSortField[] = [
  'cardNumber',
  'dateInscription',
  'description',
  'amount',
];

@Component({
  selector: 'app-account-transactions-table',
  templateUrl: './account-transactions-table.component.html',
  styleUrl: './account-transactions-table.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatTableModule, MatSortModule, MatIconButton, MatIcon, TranslatePipe],
})
export class AccountTransactionsTableComponent {
  private readonly _transactionService = inject<AccountTransactionService>(ACCOUNT_TRANSACTION_SERVICE);

  readonly accountId = input.required<string>();

  private readonly _currentPage = signal(0);
  private readonly _sort = signal<TransactionSort>(DEFAULT_SORT);
  readonly transactions = signal<AccountTransactionModel[]>([]);
  readonly totalPages = signal(0);
  readonly displayPage = computed(() => this._currentPage() + 1);
  readonly isFirstPage = computed(() => this._currentPage() === 0);
  readonly isLastPage = computed(() => this._currentPage() >= this.totalPages() - 1);
  readonly sortField = computed(() => this._sort().field);
  readonly sortDirection = computed(() => this._sort().direction);

  readonly pageSize = 25;
  readonly displayedColumns = ['cardNumber', 'dateInscription', 'description', 'amount', 'balance'];

  constructor() {
    effect(() => {
      const accountId = this.accountId();
      const page = this._currentPage();
      const sort = this._sort();
      this._transactionService.transactionsVersion();
      this._loadPage(accountId, page, sort);
    });
  }

  private async _loadPage(accountId: string, page: number, sort: TransactionSort): Promise<void> {
    const result = await this._transactionService.getPageByAccount(accountId, page, this.pageSize, sort);
    this.transactions.set(result.transactions);
    this.totalPages.set(result.totalPages);
  }

  onSortChange(event: Sort): void {
    if (event.direction === '' || !this._isSortableField(event.active)) {
      this._sort.set(DEFAULT_SORT);
    } else {
      this._sort.set({ field: event.active, direction: event.direction });
    }
    this._currentPage.set(0);
  }

  private _isSortableField(value: string): value is TransactionSortField {
    return (SORTABLE_FIELDS as readonly string[]).includes(value);
  }

  onPreviousPage(): void {
    this._currentPage.update(p => p - 1);
  }

  onNextPage(): void {
    this._currentPage.update(p => p + 1);
  }

  onGoToPage(event: Event): void {
    const input = event.target as HTMLInputElement;
    const page = parseInt(input.value, 10);
    if (isNaN(page)) return;
    const clamped = Math.max(1, Math.min(page, this.totalPages())) - 1;
    this._currentPage.set(clamped);
  }

  formatAmount(amount: number): string {
    const [intPart, decPart] = amount.toFixed(2).split('.');
    const sign = intPart.startsWith('-') ? '-' : '';
    const intAbs = sign ? intPart.slice(1) : intPart;
    const grouped = intAbs.replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
    return `${sign}${grouped}.${decPart}`;
  }

  isPositiveAmount(row: AccountTransactionModel): boolean {
    return row.recordType !== AccountTransactionRecordType.snapshot && row.amount > 0;
  }

  isNegativeAmount(row: AccountTransactionModel): boolean {
    return row.recordType !== AccountTransactionRecordType.snapshot && row.amount < 0;
  }

  formatBalance(balance: number | undefined): string {
    return balance === undefined ? '' : this.formatAmount(balance);
  }
}
