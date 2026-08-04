import { Provider } from '@angular/core';
import { environment } from '@/environments/environment';
import { ACCOUNT_TRANSACTION_SERVICE } from '@services/account-transaction.service';
import { AccountTransactionServicePwaImpl } from '@services/pwa/account-transaction-pwa.service';
import { AccountTransactionServiceServerImpl } from '@services/server/account-transaction-server.service';

export const AccountTransactionServiceProvider: Provider = {
  provide: ACCOUNT_TRANSACTION_SERVICE,
  useClass:
    environment.buildType === 'server'
      ? AccountTransactionServiceServerImpl
      : AccountTransactionServicePwaImpl,
};
