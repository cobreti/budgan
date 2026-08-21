export function parseIsoDate(iso: string): Date | null {
  const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(iso);
  if (!match) return null;
  const [, y, m, d] = match;
  return new Date(Number(y), Number(m) - 1, Number(d));
}

export function formatIsoDate(date: Date): string {
  const y = date.getFullYear().toString().padStart(4, '0');
  const m = (date.getMonth() + 1).toString().padStart(2, '0');
  const d = date.getDate().toString().padStart(2, '0');
  return `${y}-${m}-${d}`;
}

export function previousIsoDay(iso: string): string {
  const d = parseIsoDate(iso);
  if (!d) return iso;
  d.setDate(d.getDate() - 1);
  return formatIsoDate(d);
}

export interface DateFormatParseSuccess {
  success: true;
  date: Date;
}

export interface DateFormatParseError {
  success: false;
  error: 'invalid-regex' | 'no-match' | 'missing-groups' | 'invalid-date';
}

export type DateFormatParseResult = DateFormatParseSuccess | DateFormatParseError;

/**
 * Parses `value` using a regex containing named capture groups `year`,
 * `month` (1-indexed) and `day`. Never throws; unmatched/invalid input is
 * reported via the discriminated error result instead.
 */
export function parseDateWithFormatDetailed(value: string, pattern: string): DateFormatParseResult {
  let regex: RegExp;
  try {
    regex = new RegExp(pattern);
  } catch {
    return { success: false, error: 'invalid-regex' };
  }

  if (!/\(\?<year>/.test(pattern) || !/\(\?<month>/.test(pattern) || !/\(\?<day>/.test(pattern)) {
    return { success: false, error: 'missing-groups' };
  }

  const match = regex.exec(value);
  if (!match || !match.groups) {
    return { success: false, error: 'no-match' };
  }

  const { year, month, day } = match.groups;
  if (year === undefined || month === undefined || day === undefined) {
    return { success: false, error: 'missing-groups' };
  }

  const y = parseInt(year, 10);
  const m = parseInt(month, 10) - 1;
  const d = parseInt(day, 10);

  if (Number.isNaN(y) || Number.isNaN(m) || Number.isNaN(d)) {
    return { success: false, error: 'invalid-date' };
  }

  const date = new Date(y, m, d);
  if (date.getFullYear() !== y || date.getMonth() !== m || date.getDate() !== d) {
    return { success: false, error: 'invalid-date' };
  }

  return { success: true, date };
}

export function parseDateWithFormat(value: string, pattern: string): Date | null {
  const result = parseDateWithFormatDetailed(value, pattern);
  return result.success ? result.date : null;
}

export interface DateFormatPreset {
  label: string;
  pattern: string;
}

export const DATE_FORMAT_PRESETS: DateFormatPreset[] = [
  { label: 'YYYY-MM-DD', pattern: String.raw`(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})` },
  { label: 'YYYY/MM/DD', pattern: String.raw`(?<year>\d{4})/(?<month>\d{2})/(?<day>\d{2})` },
  { label: 'YYYYMMDD', pattern: String.raw`(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})` },
  { label: 'DD/MM/YYYY', pattern: String.raw`(?<day>\d{2})/(?<month>\d{2})/(?<year>\d{4})` },
  { label: 'MM/DD/YYYY', pattern: String.raw`(?<month>\d{2})/(?<day>\d{2})/(?<year>\d{4})` },
  { label: 'DD-MM-YYYY', pattern: String.raw`(?<day>\d{2})-(?<month>\d{2})-(?<year>\d{4})` },
  { label: 'MM-DD-YYYY', pattern: String.raw`(?<month>\d{2})-(?<day>\d{2})-(?<year>\d{4})` },
  { label: 'DD.MM.YYYY', pattern: String.raw`(?<day>\d{2})\.(?<month>\d{2})\.(?<year>\d{4})` },
];

/**
 * Reverse lookup used to show a friendly label (e.g. "DD/MM/YYYY") for a
 * stored regex that happens to match one of the known presets exactly.
 * Returns undefined for hand-written/custom patterns.
 */
export function findDateFormatPreset(pattern: string): DateFormatPreset | undefined {
  return DATE_FORMAT_PRESETS.find((preset) => preset.pattern === pattern);
}
