import { Injectable, inject, signal } from '@angular/core';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { EventType, InteractionStatus } from '@azure/msal-browser';
import { filter } from 'rxjs';
import { AuthService } from '@services/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthServiceServerImpl implements AuthService {
  private readonly _msalService = inject(MsalService);
  private readonly _msalBroadcastService = inject(MsalBroadcastService);

  readonly isAuthenticated = signal(this._hasActiveAccount());
  readonly accountName = signal(this._activeAccountName());

  constructor() {
    this._msalBroadcastService.msalSubject$
      .pipe(
        filter(
          (msg) =>
            msg.eventType === EventType.LOGIN_SUCCESS ||
            msg.eventType === EventType.LOGOUT_SUCCESS ||
            msg.eventType === EventType.ACQUIRE_TOKEN_SUCCESS,
        ),
      )
      .subscribe(() => this._refresh());

    this._msalBroadcastService.inProgress$
      .pipe(filter((status) => status === InteractionStatus.None))
      .subscribe(() => this._refresh());
  }

  login(): void {
    this._msalService.loginRedirect();
  }

  logout(): void {
    this._msalService.logoutRedirect();
  }

  private _refresh(): void {
    this.isAuthenticated.set(this._hasActiveAccount());
    this.accountName.set(this._activeAccountName());
  }

  private _hasActiveAccount(): boolean {
    return this._msalService.instance.getActiveAccount() !== null;
  }

  private _activeAccountName(): string | null {
    const account = this._msalService.instance.getActiveAccount();
    return account?.name ?? account?.username ?? null;
  }
}
