import { Provider } from '@angular/core';
import { environment } from '@/environments/environment';
import { ACCOUNT_SERVICE } from '@services/account.service';
import { AccountServicePwaImpl } from '@services/pwa/account-pwa.service';
import { AccountServiceServerImpl } from '@services/server/account-server.service';

export const AccountServiceProvider: Provider = {
  provide: ACCOUNT_SERVICE,
  useClass:
    environment.buildType === 'server' ? AccountServiceServerImpl : AccountServicePwaImpl,
};
