import { InjectionToken, Signal } from '@angular/core';

export interface AuthService {
  readonly isAuthenticated: Signal<boolean>;
  readonly accountName: Signal<string | null>;
  login(): void;
  logout(): void;
}

export const AUTH_SERVICE = new InjectionToken<AuthService>('AuthService');
