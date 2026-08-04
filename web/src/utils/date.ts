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

export function parseYYYYMMDD(str:string) {
  const year = parseInt(str.slice(0, 4), 10);
  const month = parseInt(str.slice(4, 6), 10) - 1; // JS months are 0-indexed
  const day = parseInt(str.slice(6, 8), 10);
  return new Date(year, month, day);
}
