import moment from 'moment';

export type MonthRange = { start: string; end: string };
export type ViewType = 'expense' | 'income' | 'all';

// Normalizes any parseable date string to a canonical 'YYYY-MM' key. Using
// moment (rather than a raw string slice) means transaction dates that
// aren't already zero-padded ISO (e.g. '2026-7-5') still produce the same
// key as the moment-formatted keys used for the month dropdown, so the two
// never silently fail to match.
export function toMonthKey(dateAsString: string): string | null {
  const parsed = moment(dateAsString);
  return parsed.isValid() ? parsed.format('YYYY-MM') : null;
}

export function monthsBetween(range: MonthRange): string[] {
  const months: string[] = [];
  const cursor = moment(range.start, 'YYYY-MM');
  const last = moment(range.end, 'YYYY-MM');
  while (cursor.isSameOrBefore(last, 'month')) {
    months.push(cursor.format('YYYY-MM'));
    cursor.add(1, 'month');
  }
  return months;
}

export function monthBounds(month: string): { start: Date; end: Date } {
  return {
    start: moment(month, 'YYYY-MM').startOf('month').toDate(),
    end: moment(month, 'YYYY-MM').endOf('month').toDate(),
  };
}
