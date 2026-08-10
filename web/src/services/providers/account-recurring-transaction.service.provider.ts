import { Provider } from '@angular/core';
import { isServerBuild } from '@/utils/build-type';
import { ACCOUNT_RECURRING_TRANSACTION_SERVICE } from '@services/account-recurring-transaction.service';
import { AccountRecurringTransactionServicePwaImpl } from '@services/pwa/account-recurring-transaction-pwa.service';
import { AccountRecurringTransactionServiceServerImpl } from '@services/server/account-recurring-transaction-server.service';

export const AccountRecurringTransactionServiceProvider: Provider = {
  provide: ACCOUNT_RECURRING_TRANSACTION_SERVICE,
  useClass:
    isServerBuild()
      ? AccountRecurringTransactionServiceServerImpl
      : AccountRecurringTransactionServicePwaImpl,
};
