import { InjectionToken } from '@angular/core';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';

export interface AccountRecurringTransactionService {
  replaceForAccount(
    accountId: string,
    transactions: AccountRecurringTransactionModel[],
  ): Promise<void>;
}

export const ACCOUNT_RECURRING_TRANSACTION_SERVICE =
  new InjectionToken<AccountRecurringTransactionService>('AccountRecurringTransactionService');
