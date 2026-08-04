import { Injectable } from '@angular/core';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';
import { AccountRecurringTransactionService } from '@services/account-recurring-transaction.service';

@Injectable({ providedIn: 'root' })
export class AccountRecurringTransactionServiceServerImpl
  implements AccountRecurringTransactionService
{
  async replaceForAccount(
    _accountId: string,
    _transactions: AccountRecurringTransactionModel[],
  ): Promise<void> {
    throw new Error('replaceForAccount is not supported in server mode');
  }
}
