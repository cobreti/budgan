import {
  DateMonthRange,
  DateRangeComponent,
} from '@/components/date-range/date-range';
import {
  AccountTransactionModel,
  AccountTransactionRecordType,
} from '@/Models/accountTransactionModel';
import {
  ACCOUNT_RECURRING_TRANSACTION_SERVICE,
  AccountRecurringTransactionService,
  StructuredTransactionsByRecurringId,
} from '@/services/account-recurring-transaction.service';
import {
  ACCOUNT_TRANSACTION_SERVICE,
  AccountTransactionService,
} from '@/services/account-transaction.service';
import {
  monthBounds,
  MonthRange,
  monthsBetween,
  toMonthKey,
} from '@/utils/recurring-month';
import { RecurringPieChartComponent } from '@/views/accounts/account-graphs/recurring-pie-chart/recurring-pie-chart.component';
import {
  ChangeDetectorRef,
  Component,
  effect,
  inject,
  signal,
} from '@angular/core';

@Component({
  selector: 'app-account-overview-graph',
  templateUrl: './account-overview-graph.html',
  styleUrl: './account-overview-graph.scss',
  imports: [RecurringPieChartComponent, DateRangeComponent],
})
export class AccountOverviewGraphComponent {
  private readonly _transactionService = inject<AccountTransactionService>(
    ACCOUNT_TRANSACTION_SERVICE
  );
  private readonly _recurringTransactionsService =
    inject<AccountRecurringTransactionService>(
      ACCOUNT_RECURRING_TRANSACTION_SERVICE
    );
  private readonly _cdr = inject(ChangeDetectorRef);

  readonly recurringTransactions = signal<StructuredTransactionsByRecurringId>(
    {}
  );
  readonly dateMonthRange = signal<DateMonthRange>({});

  protected readonly startMonth = signal<string | null>(null);
  protected readonly endMonth = signal<string | null>(null);
  private readonly _monthRange = signal<MonthRange | null>(null);

  constructor() {
    this.loadAvailableMonths();

    effect(() => {
      const { startMonth, endMonth } = this.dateMonthRange();

      this.loadRecurringTransactions(startMonth, endMonth);
    });
  }

  private async getAllTransactions(): Promise<AccountTransactionModel[]> {
    const trxs = await this._transactionService.getList();

    return trxs;
  }

  private async loadAvailableMonths(): Promise<void> {
    // Guards against out-of-order async resolution: if accountId changes
    // again before this call resolves, a later call's request id will have
    // moved on, so this (now stale) result is discarded instead of
    // overwriting the dropdowns with another account's months.
    // const requestId = ++this._availableMonthsRequestId;
    const transactions = await this.getAllTransactions();

    const matchingDates = transactions
      .filter((t) => t.recordType === AccountTransactionRecordType.normal)
      .map((t) => t.dateInscriptionAsString);

    const startDate =
      matchingDates.length > 0
        ? matchingDates.reduce((min, d) => (d < min ? d : min))
        : undefined;
    const endDate =
      matchingDates.length > 0
        ? matchingDates.reduce((max, d) => (d > max ? d : max))
        : undefined;

    if (!startDate || !endDate) {
      this._monthRange.set(null);
      this.startMonth.set(null);
      this.endMonth.set(null);
      this._cdr.markForCheck();
      return;
    }

    const startMonthKey = toMonthKey(startDate);
    const endMonthKey = toMonthKey(endDate);
    const range: MonthRange | null =
      startMonthKey && endMonthKey
        ? { start: startMonthKey, end: endMonthKey }
        : null;
    this._monthRange.set(range);

    const months = range ? monthsBetween(range) : [];

    // Default to the full available span; keep an existing selection if it's
    // still valid within the (possibly changed) range, otherwise reset it.
    const currentStart = this.startMonth();
    const currentEnd = this.endMonth();
    const validStart =
      currentStart && months.includes(currentStart)
        ? currentStart
        : (months[0] ?? null);
    const validEnd =
      currentEnd && months.includes(currentEnd)
        ? currentEnd
        : (months[months.length - 1] ?? null);

    this.startMonth.set(validStart);
    this.endMonth.set(
      validStart && validEnd && validEnd < validStart ? validStart : validEnd
    );

    this._cdr.markForCheck();
  }

  private async loadRecurringTransactions(
    startMonth: string | undefined,
    endMonth: string | undefined
  ): Promise<void> {
    if (!startMonth || !endMonth) {
      this.recurringTransactions.set({});
      return;
    }

    const start = monthBounds(startMonth).start;
    const end = monthBounds(endMonth).end;

    const trxs =
      await this._recurringTransactionsService.getAllStructuredRecurringTransactions(
        start,
        end
      );

    this.recurringTransactions.set(trxs);
  }
}
