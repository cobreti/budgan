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
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { BaseChartDirective } from 'ng2-charts';
import type { ChartData, ChartOptions, TooltipItem } from 'chart.js';
import moment from 'moment';
import {
  ACCOUNT_TRANSACTION_SERVICE,
  AccountTransactionService,
} from '@services/account-transaction.service';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import {
  AccountTransactionModel,
  AccountTransactionRecordType,
} from '@models/accountTransactionModel';
import { formatIsoDate, previousIsoDay } from '@/utils/date';
import { monthBounds } from '@/utils/recurring-month';

type DataPoint = {
  date: string;
  balance: number;
  isSnapshot?: boolean;
  description?: string;
  amount?: number;
};

@Component({
  selector: 'app-balance-trend-graph',
  templateUrl: './balance-trend-graph.component.html',
  styleUrl: './balance-trend-graph.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TranslatePipe, BaseChartDirective],
})
export class BalanceTrendGraphComponent {
  private readonly _transactionService = inject<AccountTransactionService>(
    ACCOUNT_TRANSACTION_SERVICE,
  );
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _translate = inject(TranslateService);
  private readonly _cdr = inject(ChangeDetectorRef);

  readonly accountId = input.required<string>();
  readonly startMonth = input.required<string | null>();
  readonly endMonth = input.required<string | null>();

  protected readonly _allPoints = signal<DataPoint[]>([]);

  // Windows the full balance history to the selected range. The point carried
  // in from just before the range start anchors the line so it doesn't start
  // from a visual discontinuity — it reflects the real balance entering the
  // range, not a reset to zero.
  protected readonly _points = computed<DataPoint[]>(() => {
    const all = this._allPoints();
    const startMonth = this.startMonth();
    const endMonth = this.endMonth();
    if (!startMonth || !endMonth) return all;

    const startIso = formatIsoDate(monthBounds(startMonth).start);
    const endIso = formatIsoDate(monthBounds(endMonth).end);

    const withinRange = all.filter((p) => p.date >= startIso && p.date <= endIso);
    const before = all.filter((p) => p.date < startIso);
    const carryIn = before.length > 0 ? before[before.length - 1] : null;

    if (!carryIn || withinRange[0]?.date === startIso) return withinRange;

    return [{ date: startIso, balance: carryIn.balance }, ...withinRange];
  });

  // A full, unscoped history needs 2+ points to read as a "trend" at all.
  // Once zoomed to a range, though, even one point is real data (just a
  // quiet stretch) rather than an absence of data — so the bar to display
  // something is lower.
  readonly hasData = computed<boolean>(() => {
    const pts = this._points();
    return this.startMonth() && this.endMonth() ? pts.length >= 1 : pts.length >= 2;
  });

  readonly chartData = computed<ChartData<'line'>>(() => {
    const pts = this._points();
    return {
      labels: pts.map((p) => p.date), // MM-DD
      datasets: [
        {
          data: pts.map((p) => p.balance),
          fill: true,
          tension: 0.1,
          // Per-point styling: snapshot gets a larger amber dot.
          // Scriptable functions are evaluated independently per point — unlike arrays,
          // they have no carry-forward behaviour for undefined values.
          pointRadius: (ctx) => (pts[ctx.dataIndex]?.isSnapshot ? 3 : 3),
          pointBackgroundColor: (ctx) =>
            pts[ctx.dataIndex]?.isSnapshot ? 'rgba(255, 152, 0, 1)' : 'rgba(100, 100, 200, 1)',
          pointBorderColor: (ctx) =>
            pts[ctx.dataIndex]?.isSnapshot ? 'rgba(255, 152, 0, 1)' : 'rgba(100, 100, 200, 1)',
          pointBorderWidth: (ctx) => (pts[ctx.dataIndex]?.isSnapshot ? 1 : 1),
        },
      ],
    };
  });

  readonly chartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          // Reads from _points() (rather than the closure captured when
          // chartData was computed) so the tooltip always reflects the
          // range/filter currently applied, since chartOptions itself is a
          // static field and isn't recreated on every recompute.
          title: (items: TooltipItem<'line'>[]) => {
            const point = this._points()[items[0]?.dataIndex];
            return point ? this._formatTooltipDate(point.date) : '';
          },
          label: (item: TooltipItem<'line'>) => {
            const point = this._points()[item.dataIndex];
            if (!point) return '';
            const lines: string[] = [];
            if (point.isSnapshot) {
              lines.push(this._translate.instant('balanceTrendGraph.snapshot'));
            } else if (point.description !== undefined) {
              lines.push(point.description);
              lines.push(
                `${this._translate.instant('balanceTrendGraph.amount')}: ${this._formatAmount(point.amount ?? 0)}`,
              );
            } else {
              lines.push(this._translate.instant('balanceTrendGraph.openingBalance'));
            }
            lines.push(
              `${this._translate.instant('balanceTrendGraph.cumulatedBalance')}: ${this._formatAmount(point.balance)}`,
            );
            return lines;
          },
        },
      },
    },
    scales: {
      x: {
        ticks: { maxTicksLimit: 5, maxRotation: 0 },
      },
      y: {
        ticks: { maxTicksLimit: 5 },
      },
    },
  };

  constructor() {
    effect(() => {
      const id = this.accountId();
      // Track transactionsVersion so this effect re-runs when balances are recalculated
      this._transactionService.transactionsVersion();
      this._loadPoints(id);
    });
  }

  private _formatAmount(amount: number): string {
    const [intPart, decPart] = amount.toFixed(2).split('.');
    const sign = intPart.startsWith('-') ? '-' : '';
    const intAbs = sign ? intPart.slice(1) : intPart;
    const grouped = intAbs.replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
    return `${sign}${grouped}.${decPart}`;
  }

  // Points' dates come from either dashed ("YYYY-MM-DD", normal transactions)
  // or dashless ("YYYYMMDD", snapshot) storage formats — moment is given both
  // so it can parse either without guessing wrong on one of them.
  private _formatTooltipDate(date: string): string {
    return moment(date, ['YYYY-MM-DD', 'YYYYMMDD'])
      .locale(this._locale.currentLocale())
      .format('LL');
  }

  private async _loadPoints(accountId: string): Promise<void> {
    const txs = await this._transactionService.getListByAccount(accountId);

    // Keep only normal transactions that have a computed balance, sorted chronologically
    const normal: AccountTransactionModel[] = txs
      .filter(
        (t) => t.recordType === AccountTransactionRecordType.normal && t.balance !== undefined,
      )
      .sort((a, b) => {
        const d = a.dateInscriptionAsString.localeCompare(b.dateInscriptionAsString);
        if (d !== 0) return d;
        return (a.balanceDateOffset ?? 0) - (b.balanceDateOffset ?? 0);
      });

    this._allPoints.set(this._buildBalancePoints(txs, normal));
    this._cdr.markForCheck();
  }

  private _buildBalancePoints(
    txs: AccountTransactionModel[],
    normal: AccountTransactionModel[],
  ): DataPoint[] {
    if (normal.length === 0) return [];

    // Opening point: the balance one day before the earliest transaction, derived
    // from that transaction's own computed balance (works whether or not a
    // snapshot anchors the account — recalculateBalances() sets `balance` either way).
    const earliest = normal[0];
    const opening: DataPoint = {
      date: previousIsoDay(earliest.dateInscriptionAsString),
      balance: (earliest.balance ?? 0) - earliest.amount,
    };

    // Build remaining points: normal transactions + the snapshot itself (if present).
    // The snapshot has balanceDateOffset === 0, so it sorts between pre-snapshot
    // transactions (negative offset) and on-or-after-snapshot transactions (positive offset).
    const snapshotTx = txs.find((t) => t.recordType === AccountTransactionRecordType.snapshot);
    const points: DataPoint[] = normal.map((t) => ({
      date: t.dateInscriptionAsString,
      balance: t.balance!,
      description: t.description,
      amount: t.amount,
    }));

    if (snapshotTx) {
      points.push({
        date: snapshotTx.dateInscriptionAsString,
        balance: snapshotTx.amount,
        isSnapshot: true,
      });
      points.sort((a, b) => {
        const d = a.date.localeCompare(b.date);
        if (d !== 0) return d;
        // On the same date: snapshot (isSnapshot) sits between negative-offset
        // and positive-offset normal rows — sort it to offset 0 position.
        if (a.isSnapshot) return -1;
        if (b.isSnapshot) return 1;
        return 0;
      });
    }

    return [opening, ...points];
  }
}
