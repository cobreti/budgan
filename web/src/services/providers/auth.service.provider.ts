import { Provider } from '@angular/core';
import { isServerBuild } from '@/utils/build-type';
import { AUTH_SERVICE } from '@services/auth.service';
import { AuthServiceNoopImpl } from '@services/pwa/auth-pwa.service';
import { AuthServiceServerImpl } from '@services/server/auth-server.service';

export const AuthServiceProvider: Provider = {
  provide: AUTH_SERVICE,
  useClass: isServerBuild() ? AuthServiceServerImpl : AuthServiceNoopImpl,
};
