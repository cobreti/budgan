import { Injectable, InjectionToken, Signal, signal } from '@angular/core';

export interface ThemeService {
  readonly isDark: Signal<boolean>;
  toggle(): void;
}

export const THEME_SERVICE = new InjectionToken<ThemeService>('ThemeService');

@Injectable({ providedIn: 'root' })
export class ThemeServiceImpl implements ThemeService {
  private static readonly STORAGE_KEY = 'theme';

  private readonly _isDark = signal(
    localStorage.getItem(ThemeServiceImpl.STORAGE_KEY) === 'dark'
  );

  readonly isDark: Signal<boolean> = this._isDark;

  constructor() {
    document.documentElement.classList.toggle('dark-theme', this._isDark());
  }

  toggle(): void {
    const next = !this._isDark();
    this._isDark.set(next);
    localStorage.setItem(ThemeServiceImpl.STORAGE_KEY, next ? 'dark' : 'light');
    document.documentElement.classList.toggle('dark-theme', next);
  }
}
