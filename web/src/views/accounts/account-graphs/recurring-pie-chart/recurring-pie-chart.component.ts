import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  computed,
  effect,
  inject,
  input,
  signal,
} from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { BaseChartDirective } from 'ng2-charts';
import type { ChartData, ChartOptions, LegendItem } from 'chart.js';
import { MatOption } from '@angular/material/core';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatSelect, MatSelectChange, MatSelectTrigger } from '@angular/material/select';
import {
  ACCOUNT_TRANSACTION_SERVICE,
  AccountTransactionService,
} from '@services/account-transaction.service';
import { monthBounds, ViewType } from '@/utils/recurring-month';

type RecurringSlice = { description: string; amount: number };

@Component({
  selector: 'app-recurring-pie-chart',
  templateUrl: './recurring-pie-chart.component.html',
  styleUrl: './recurring-pie-chart.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    TranslatePipe,
    BaseChartDirective,
    MatFormField,
    MatLabel,
    MatSelect,
    MatSelectTrigger,
    MatOption,
  ],
})
export class RecurringPieChartComponent {
  private readonly _transactionService = inject<AccountTransactionService>(
    ACCOUNT_TRANSACTION_SERVICE,
  );
  private readonly _cdr = inject(ChangeDetectorRef);

  readonly accountId = input.required<string>();
  readonly startMonth = input.required<string | null>();
  readonly endMonth = input.required<string | null>();

  protected readonly viewTypes: ViewType[] = ['all', 'expense', 'income'];
  protected readonly viewType = signal<ViewType>('all');

  protected readonly slices = signal<RecurringSlice[]>([]);

  // Indices manually hidden by clicking a legend entry (Chart.js toggles a
  // pie slice's visibility per data index, not per dataset). Kept in sync
  // with the chart via chartOptions.plugins.legend.onClick below.
  protected readonly hiddenIndices = signal<ReadonlySet<number>>(new Set());

  readonly chartData = computed<ChartData<'pie'>>(() => {
    const slices = this.slices();
    return {
      labels: slices.map((s) => s.description),
      datasets: [{ data: slices.map((s) => s.amount) }],
    };
  });

  // Complete total always includes every detected recurring item, regardless
  // of what the user has hidden from the chart's legend.
  readonly completeTotal = computed(() => this.slices().reduce((sum, s) => sum + s.amount, 0));

  // Calculated total only counts slices still visible on the chart.
  readonly visibleTotal = computed(() => {
    const hidden = this.hiddenIndices();
    if (hidden.size === 0) return this.completeTotal();
    return this.slices().reduce((sum, s, i) => (hidden.has(i) ? sum : sum + s.amount), 0);
  });

  readonly chartOptions: ChartOptions<'pie'> = {
    responsive: true,
    // The chart fills whatever box its container is given (see
    // .recurring-pie-chart__chart-container) instead of deriving its own
    // height from an aspect ratio, so it never grows past the card's bounds.
    maintainAspectRatio: false,
    plugins: {
      // 'right' stacks the legend as a vertical list beside the chart. Items
      // follow the dataset's own order, which `slices` already sorts by
      // amount descending, so the legend reads highest-to-lowest top to bottom.
      legend: {
        position: 'right',
        align: 'start',
        // Reimplements Chart.js's default pie/doughnut legend click (toggle
        // the data index's visibility) so we can mirror the resulting hidden
        // state into hiddenIndices() for the total calculation.
        onClick: (_event, legendItem: LegendItem, legend) => {
          const index = legendItem.index;
          if (index === undefined) return;
          const chart = legend.chart;
          chart.toggleDataVisibility(index);
          chart.update();
          this._setIndexHidden(index, !chart.getDataVisibility(index));
        },
      },
    },
  };

  constructor() {
    // Recomputed whenever the account, view type, or selected range changes.
    effect(() => {
      const id = this.accountId();
      const startMonth = this.startMonth();
      const endMonth = this.endMonth();
      const viewType = this.viewType();
      this._transactionService.transactionsVersion();
      this._loadSlicesForRange(id, startMonth, endMonth, viewType);
    });
  }

  viewTypeLabelKey(viewType: ViewType): string {
    if (viewType === 'expense') return 'recurringPieChart.expenses';
    if (viewType === 'income') return 'recurringPieChart.incomes';
    return 'recurringPieChart.all';
  }

  onViewTypeChange(change: MatSelectChange): void {
    this.viewType.set(change.value as ViewType);
  }

  formatTotal(amount: number): string {
    const [intPart, decPart] = amount.toFixed(2).split('.');
    const sign = intPart.startsWith('-') ? '-' : '';
    const intAbs = sign ? intPart.slice(1) : intPart;
    const grouped = intAbs.replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
    return `${sign}${grouped}.${decPart}`;
  }

  private _setIndexHidden(index: number, hidden: boolean): void {
    this.hiddenIndices.update((prev) => {
      if (hidden === prev.has(index)) return prev;
      const next = new Set(prev);
      if (hidden) next.add(index);
      else next.delete(index);
      return next;
    });
    this._cdr.markForCheck();
  }

  private async _loadSlicesForRange(
    accountId: string,
    startMonth: string | null,
    endMonth: string | null,
    viewType: ViewType,
  ): Promise<void> {
    if (!startMonth || !endMonth) {
      this.slices.set([]);
      this.hiddenIndices.set(new Set());
      this._cdr.markForCheck();
      return;
    }

    const start = monthBounds(startMonth).start;
    const end = monthBounds(endMonth).end;
    const txs = await this._transactionService.getRecurringTransactionsByAccount(
      accountId,
      start,
      end,
    );

    // getRecurringTransactionsByAccount returns both signs; keep only the
    // ones matching the selected view type ('all' keeps both).
    const matchingTxs = txs.filter((t) => {
      if (viewType === 'expense') return t.amount < 0;
      if (viewType === 'income') return t.amount > 0;
      return true;
    });

    const totals = new Map<string, number>();
    for (const t of matchingTxs) {
      totals.set(t.description, (totals.get(t.description) ?? 0) + Math.abs(t.amount));
    }

    const slices = [...totals.entries()]
      .map(([description, amount]) => ({ description, amount }))
      .sort((a, b) => b.amount - a.amount);

    this.slices.set(slices);
    this.hiddenIndices.set(new Set());
    this._cdr.markForCheck();
  }
}
