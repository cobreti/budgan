import { findDateFormatPreset, parseDateWithFormat, parseDateWithFormatDetailed } from './date';

describe('parseDateWithFormatDetailed', () => {
  it('parses YYYYMMDD-style values', () => {
    const result = parseDateWithFormatDetailed(
      '20240315',
      '(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})',
    );
    expect(result).toEqual({ success: true, date: new Date(2024, 2, 15) });
  });

  it('parses DD/MM/YYYY values', () => {
    const result = parseDateWithFormatDetailed(
      '15/03/2024',
      '(?<day>\\d{2})/(?<month>\\d{2})/(?<year>\\d{4})',
    );
    expect(result).toEqual({ success: true, date: new Date(2024, 2, 15) });
  });

  it('parses MM-DD-YYYY values', () => {
    const result = parseDateWithFormatDetailed(
      '03-15-2024',
      '(?<month>\\d{2})-(?<day>\\d{2})-(?<year>\\d{4})',
    );
    expect(result).toEqual({ success: true, date: new Date(2024, 2, 15) });
  });

  it('parses YYYY.MM.DD values', () => {
    const result = parseDateWithFormatDetailed(
      '2024.03.15',
      '(?<year>\\d{4})\\.(?<month>\\d{2})\\.(?<day>\\d{2})',
    );
    expect(result).toEqual({ success: true, date: new Date(2024, 2, 15) });
  });

  it('reports invalid-regex for a pattern that does not compile', () => {
    const result = parseDateWithFormatDetailed('20240315', '(?<year>\\d{4');
    expect(result).toEqual({ success: false, error: 'invalid-regex' });
  });

  it('reports missing-groups when a required named group is absent', () => {
    const result = parseDateWithFormatDetailed(
      '20240315',
      '(?<year>\\d{4})(?<month>\\d{2})(\\d{2})',
    );
    expect(result).toEqual({ success: false, error: 'missing-groups' });
  });

  it('reports no-match when the value does not match the pattern', () => {
    const result = parseDateWithFormatDetailed(
      'not-a-date',
      '(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})',
    );
    expect(result).toEqual({ success: false, error: 'no-match' });
  });

  it('reports invalid-date for an out-of-range day (rollover guard)', () => {
    const result = parseDateWithFormatDetailed(
      '20240230',
      '(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})',
    );
    expect(result).toEqual({ success: false, error: 'invalid-date' });
  });

  it('reports invalid-date for an out-of-range month', () => {
    const result = parseDateWithFormatDetailed(
      '20241315',
      '(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})',
    );
    expect(result).toEqual({ success: false, error: 'invalid-date' });
  });
});

describe('parseDateWithFormat', () => {
  it('returns a Date on success', () => {
    const date = parseDateWithFormat('20240315', '(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})');
    expect(date).toEqual(new Date(2024, 2, 15));
  });

  it('returns null on failure', () => {
    const date = parseDateWithFormat('nope', '(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})');
    expect(date).toBeNull();
  });
});

describe('findDateFormatPreset', () => {
  it('returns the matching preset for a known pattern', () => {
    const preset = findDateFormatPreset(
      '(?<day>\\d{2})/(?<month>\\d{2})/(?<year>\\d{4})',
    );
    expect(preset?.label).toBe('DD/MM/YYYY');
  });

  it('returns undefined for a custom pattern with no matching preset', () => {
    const preset = findDateFormatPreset('(?<year>\\d{4})_(?<month>\\d{2})_(?<day>\\d{2})');
    expect(preset).toBeUndefined();
  });
});
