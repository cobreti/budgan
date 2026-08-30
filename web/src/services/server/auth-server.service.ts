import { Injectable, inject, signal } from '@angular/core';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { AuthenticationResult, EventType, InteractionStatus } from '@azure/msal-browser';
import { filter } from 'rxjs';
import { AuthService } from '@services/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthServiceServerImpl implements AuthService {
  private readonly _msalService = inject(MsalService);
  private readonly _msalBroadcastService = inject(MsalBroadcastService);

  readonly isAuthenticated = signal(this._hasActiveAccount());
  readonly accountName = signal(this._activeAccountName());
  readonly username = signal(this._activeUsername());
  readonly oid = signal(this._activeOid());
  readonly roles = signal(this._activeRoles());

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
      .subscribe((msg) => {
        if (msg.eventType === EventType.LOGIN_SUCCESS) {
          const result = msg.payload as AuthenticationResult;
          if (result.account) {
            this._msalService.instance.setActiveAccount(result.account);
          }
        }
        this._refresh();
      });

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
    this.username.set(this._activeUsername());
    this.oid.set(this._activeOid());
    this.roles.set(this._activeRoles());
  }

  private _hasActiveAccount(): boolean {
    return this._msalService.instance.getActiveAccount() !== null;
  }

  private _activeAccountName(): string | null {
    const account = this._msalService.instance.getActiveAccount();
    return account?.name ?? account?.username ?? null;
  }

  private _activeUsername(): string | null {
    const account = this._msalService.instance.getActiveAccount();
    return account?.idTokenClaims?.preferred_username ?? account?.username ?? null;
  }

  private _activeOid(): string | null {
    const account = this._msalService.instance.getActiveAccount();
    return account?.idTokenClaims?.oid ?? null;
  }

  private _activeRoles(): string[] {
    const account = this._msalService.instance.getActiveAccount();
    return account?.idTokenClaims?.roles ?? [];
  }
}
