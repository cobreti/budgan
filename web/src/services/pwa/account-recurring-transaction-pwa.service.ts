import { inject, Injectable } from '@angular/core';
import { IndexdbService } from '@services/indexdb.service';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';
import { AccountRecurringTransactionService } from '@services/account-recurring-transaction.service';

@Injectable({ providedIn: 'root' })
export class AccountRecurringTransactionServicePwaImpl implements AccountRecurringTransactionService {
  private readonly _indexDb = inject(IndexdbService);

  async replaceForAccount(
    accountId: string,
    transactions: AccountRecurringTransactionModel[],
  ): Promise<void> {
    await this._indexDb.recurringTransactionsTable.where('accountId').equals(accountId).delete();

    if (transactions.length > 0) {
      await this._indexDb.recurringTransactionsTable.bulkAdd(transactions);
    }
  }
}
