import { Injectable, signal } from '@angular/core';
import { AuthService } from '@services/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthServiceNoopImpl implements AuthService {
  readonly isAuthenticated = signal(true);
  readonly accountName = signal<string | null>(null);

  login(): void {}

  logout(): void {}
}
