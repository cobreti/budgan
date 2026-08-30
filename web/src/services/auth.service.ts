import { InjectionToken, Signal } from '@angular/core';

export interface AuthService {
  readonly isAuthenticated: Signal<boolean>;
  readonly accountName: Signal<string | null>;
  readonly username: Signal<string | null>;
  readonly oid: Signal<string | null>;
  readonly roles: Signal<string[]>;
  login(): void;
  logout(): void;
}

export const AUTH_SERVICE = new InjectionToken<AuthService>('AuthService');
