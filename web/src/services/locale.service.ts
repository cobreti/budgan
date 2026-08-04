import { Injectable, InjectionToken, Signal, signal } from '@angular/core';

export interface LocaleService {
  readonly currentLocale: Signal<string>;
  readonly supportedLocales: readonly string[];
  setLocale(locale: string): void;
  detectBrowserLocale(): string;
}

export const LOCALE_SERVICE = new InjectionToken<LocaleService>('LocaleService');

@Injectable({ providedIn: 'root' })
export class LocaleServiceImpl implements LocaleService {
  readonly supportedLocales = ['en', 'fr'] as const;
  private readonly _currentLocale = signal('en');

  get currentLocale(): Signal<string> { return this._currentLocale; }

  setLocale(locale: string): void {
    this._currentLocale.set(locale);
  }

  detectBrowserLocale(): string {
    const lang = navigator.language.toLowerCase();
    return this.supportedLocales.find(l => lang.startsWith(l)) ?? 'en';
  }
}
