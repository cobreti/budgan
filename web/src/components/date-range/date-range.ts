import { LOCALE_SERVICE, LocaleService } from '@/services/locale.service';
import { MonthRange, monthsBetween, toMonthKey } from '@/utils/recurring-month';
import {
  Component,
  computed,
  effect,
  inject,
  input,
  model,
  signal,
} from '@angular/core';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import {
  MatOption,
  MatSelect,
  MatSelectChange,
} from '@angular/material/select';
import { TranslatePipe } from '@ngx-translate/core';
import moment from 'moment';

export type DateMonthRange = {
  startMonth?: string;
  endMonth?: string;
};

@Component({
  selector: 'app-date-range',
  templateUrl: './date-range.html',
  styleUrl: './date-range.scss',
  imports: [MatFormField, MatLabel, MatSelect, MatOption, TranslatePipe],
})
export class DateRangeComponent {
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);

  readonly startMonth = input<string | null>(null);
  readonly endMonth = input<string | null>(null);
  private readonly _monthRange = signal<MonthRange | null>(null);

  readonly selectedStartMonth = signal<string | null>(null);
  readonly selectedEndMonth = signal<string | null>(null);

  readonly dateMonthRange = model<DateMonthRange>({});

  readonly availableMonths = computed(() => {
    const range = this._monthRange();
    if (!range) return [];
    return monthsBetween(range);
  });

  constructor() {
    effect(() => {
      const startMonth = this.startMonth();
      const endMonth = this.endMonth();

      const selectedStartMonth = this.selectedStartMonth();
      const selectedEndMonth = this.selectedEndMonth();

      if (
        !selectedStartMonth ||
        (startMonth && selectedStartMonth < startMonth)
      ) {
        this.selectedStartMonth.set(startMonth);
      }

      if (!selectedEndMonth || (endMonth && selectedEndMonth > endMonth)) {
        this.selectedEndMonth.set(endMonth);
      }
    });

    effect(() => {
      const startMonth = this.startMonth();
      const endMonth = this.endMonth();

      if (startMonth && endMonth) {
        this.calculateMonthRange(startMonth, endMonth);
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
    this.selectedStartMonth.set(value);
    const end = this.selectedEndMonth();
    if (end && value > end) {
      this.selectedEndMonth.set(value);
    }

    this.updateModel();
  }

  async onEndMonthChange(change: MatSelectChange): Promise<void> {
    const value = change.value as string;
    this.selectedEndMonth.set(value);
    const start = this.selectedStartMonth();
    if (start && value < start) {
      this.selectedStartMonth.set(value);
    }

    this.updateModel();
  }

  updateModel() {
    const startMonth = this.selectedStartMonth();
    const endMonth = this.selectedEndMonth();

    if (!startMonth || !endMonth) {
      return;
    }

    this.dateMonthRange.set({
      startMonth: startMonth,
      endMonth: endMonth,
    });
  }

  calculateMonthRange(startMonth: string, endMonth: string): void {
    const startDate = startMonth;
    const endDate = endMonth;

    const startMonthKey = toMonthKey(startDate);
    const endMonthKey = toMonthKey(endDate);
    const range: MonthRange | null =
      startMonthKey && endMonthKey
        ? { start: startMonthKey, end: endMonthKey }
        : null;
    this._monthRange.set(range);
  }
}
