import { Provider } from '@angular/core';
import { environment } from '@/environments/environment';
import { ACCOUNT_RECURRING_TRANSACTION_SERVICE } from '@services/account-recurring-transaction.service';
import { AccountRecurringTransactionServicePwaImpl } from '@services/pwa/account-recurring-transaction-pwa.service';
import { AccountRecurringTransactionServiceServerImpl } from '@services/server/account-recurring-transaction-server.service';

export const AccountRecurringTransactionServiceProvider: Provider = {
  provide: ACCOUNT_RECURRING_TRANSACTION_SERVICE,
  useClass:
    environment.buildType === 'server'
      ? AccountRecurringTransactionServiceServerImpl
      : AccountRecurringTransactionServicePwaImpl,
};
