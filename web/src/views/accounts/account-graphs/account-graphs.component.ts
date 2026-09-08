import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  effect,
  inject,
  input,
  signal,
} from '@angular/core';
import moment from 'moment';
import { MatSelectChange } from '@angular/material/select';
import {
  MatAccordion,
  MatExpansionPanel,
  MatExpansionPanelHeader,
  MatExpansionPanelTitle,
} from '@angular/material/expansion';
import { TranslatePipe } from '@ngx-translate/core';
import {
  ACCOUNT_TRANSACTION_SERVICE,
  AccountTransactionService,
} from '@services/account-transaction.service';
import { AccountTransactionRecordType } from '@models/accountTransactionModel';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import {
  monthBounds,
  MonthRange,
  monthsBetween,
  toMonthKey,
} from '@/utils/recurring-month';
import { BalanceTrendGraphComponent } from '@views/accounts/account-graphs/balance-trend-graph/balance-trend-graph.component';
import { RecurringPieChartComponent } from '@views/accounts/account-graphs/recurring-pie-chart/recurring-pie-chart.component';
import {
  ACCOUNT_RECURRING_TRANSACTION_SERVICE,
  AccountRecurringTransactionService,
  StructuredTransactionsByRecurringId,
} from '@/services/account-recurring-transaction.service';
import {
  DateMonthRange,
  DateRangeComponent,
} from '@/components/date-range/date-range';

@Component({
  selector: 'app-account-graphs',
  templateUrl: './account-graphs.component.html',
  styleUrl: './account-graphs.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    BalanceTrendGraphComponent,
    RecurringPieChartComponent,
    MatAccordion,
    MatExpansionPanel,
    MatExpansionPanelHeader,
    MatExpansionPanelTitle,
    TranslatePipe,
    DateRangeComponent,
  ],
})
export class AccountGraphsComponent {
  private readonly _transactionService = inject<AccountTransactionService>(
    ACCOUNT_TRANSACTION_SERVICE
  );
  private readonly _recurringTransactionService =
    inject<AccountRecurringTransactionService>(
      ACCOUNT_RECURRING_TRANSACTION_SERVICE
    );
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _cdr = inject(ChangeDetectorRef);

  readonly accountId = input.required<string>();
  readonly dateMonthRange = signal<DateMonthRange>({});

  protected readonly startMonth = signal<string | null>(null);
  protected readonly endMonth = signal<string | null>(null);
  // private readonly _monthRange = signal<MonthRange | null>(null);
  private _availableMonthsRequestId = 0;

  readonly recurringTransactions = signal<StructuredTransactionsByRecurringId>(
    {}
  );

  constructor() {
    // Discovers which months are selectable and picks a default month, from
    // the account's full transaction history — so the pickers stay available
    // and in sync with what the graphs below actually show.
    effect(() => {
      const id = this.accountId();
      this._transactionService.transactionsVersion();
      this._loadAvailableMonths(id);
    });

    effect(async () => {
      const id = this.accountId();
      const { startMonth, endMonth } = this.dateMonthRange();

      if (startMonth && endMonth) {
        await this.updateRecurringTransactions(id, startMonth, endMonth);
      }
    });
  }

  monthLabel(month: string): string {
    return moment(month, 'YYYY-MM')
      .locale(this._locale.currentLocale())
      .format('MMMM YYYY');
  }

  async onStartMonthChange(change: MatSelectChange): Promise<void> {
    const value = change.value as string;
    this.startMonth.set(value);
    const end = this.endMonth();
    if (end && value > end) {
      this.endMonth.set(value);
    }
  }

  async onEndMonthChange(change: MatSelectChange): Promise<void> {
    const value = change.value as string;
    this.endMonth.set(value);
    const start = this.startMonth();
    if (start && value < start) {
      this.startMonth.set(value);
    }
  }

  private async updateRecurringTransactions(
    accountId: string,
    startMonth: string,
    endMonth: string
  ): Promise<void> {
    const start = monthBounds(startMonth).start;
    const end = monthBounds(endMonth).end;

    const recurringTrxs =
      await this._recurringTransactionService.getStructuredRecurringTransactionsByAccount(
        accountId,
        start,
        end
      );
    this.recurringTransactions.set(recurringTrxs);
  }

  private async _loadAvailableMonths(accountId: string): Promise<void> {
    // Guards against out-of-order async resolution: if accountId changes
    // again before this call resolves, a later call's request id will have
    // moved on, so this (now stale) result is discarded instead of
    // overwriting the dropdowns with another account's months.
    const requestId = ++this._availableMonthsRequestId;
    const transactions =
      await this._transactionService.getListByAccount(accountId);
    if (requestId !== this._availableMonthsRequestId) return;

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
}
