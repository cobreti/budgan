import { inject, Injectable } from '@angular/core';
import { IndexdbService } from '@services/indexdb.service';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';
import {
  AccountTransactionModel,
  AccountTransactionRecordType,
} from '@models/accountTransactionModel';
import { formatIsoDate, parseIsoDate } from '@/utils/date';
import {
  AccountRecurringTransactionService,
  RecurringTransactionsSpan,
  StructuredTransactionsByRecurringId,
} from '@services/account-recurring-transaction.service';

@Injectable({ providedIn: 'root' })
export class AccountRecurringTransactionServicePwaImpl implements AccountRecurringTransactionService {
  private readonly _indexDb = inject(IndexdbService);

  async replaceForAccount(
    accountId: string,
    transactions: AccountRecurringTransactionModel[]
  ): Promise<void> {
    await this._indexDb.recurringTransactionsTable
      .where('accountId')
      .equals(accountId)
      .delete();

    if (transactions.length > 0) {
      await this._indexDb.recurringTransactionsTable.bulkAdd(transactions);
    }
  }

  async getRecurringTransactionsByAccount(
    accountId: string,
    startDate: Date,
    endDate: Date
  ): Promise<AccountTransactionModel[]> {
    const startDateAsString = formatIsoDate(startDate);
    const endDateAsString = formatIsoDate(endDate);

    const recurringPatterns = await this._indexDb.recurringTransactionsTable
      .where('accountId')
      .equals(accountId)
      .toArray();
    const recurringIds = new Set(recurringPatterns.map((r) => r.id));

    const transactions = await this._indexDb.accountTransactionsTable
      .where('accountId')
      .equals(accountId)
      .toArray();

    return transactions
      .filter(
        (t) =>
          t.recordType === AccountTransactionRecordType.normal &&
          recurringIds.has(t.recurringId) &&
          t.dateInscriptionAsString >= startDateAsString &&
          t.dateInscriptionAsString <= endDateAsString
      )
      .sort((a, b) =>
        a.dateInscriptionAsString.localeCompare(b.dateInscriptionAsString)
      );
  }

  async getStructuredRecurringTransactionsByAccount(
    accountId: string,
    startDate: Date,
    endDate: Date
  ): Promise<StructuredTransactionsByRecurringId> {
    const startDateAsString = formatIsoDate(startDate);
    const endDateAsString = formatIsoDate(endDate);

    const recurringPatterns = await this._indexDb.recurringTransactionsTable
      .where('accountId')
      .equals(accountId)
      .toArray();
    const recurringIds = new Set(recurringPatterns.map((r) => r.id));

    const transactions = await this._indexDb.accountTransactionsTable
      .where('accountId')
      .equals(accountId)
      .toArray();

    const filteredTransactions = transactions
      .filter(
        (t) =>
          t.recordType === AccountTransactionRecordType.normal &&
          recurringIds.has(t.recurringId) &&
          t.dateInscriptionAsString >= startDateAsString &&
          t.dateInscriptionAsString <= endDateAsString
      )
      .sort((a, b) =>
        a.dateInscriptionAsString.localeCompare(b.dateInscriptionAsString)
      );

    const structured: Record<string, AccountTransactionModel[]> = {};
    for (const t of filteredTransactions) {
      if (!structured[t.recurringId]) {
        structured[t.recurringId] = [];
      }
      structured[t.recurringId].push(t);
    }

    return structured;
  }

  async getRecurringTransactionsSpan(
    accountId: string
  ): Promise<RecurringTransactionsSpan | undefined> {
    const recurringPatterns = await this._indexDb.recurringTransactionsTable
      .where('accountId')
      .equals(accountId)
      .toArray();
    const recurringIds = new Set(recurringPatterns.map((r) => r.id));

    const transactions = await this._indexDb.accountTransactionsTable
      .where('accountId')
      .equals(accountId)
      .toArray();

    const dates = transactions
      .filter(
        (t) =>
          t.recordType === AccountTransactionRecordType.normal &&
          recurringIds.has(t.recurringId)
      )
      .map((t) => t.dateInscriptionAsString);

    if (dates.length === 0) return undefined;

    const start = parseIsoDate(dates.reduce((min, d) => (d < min ? d : min)));
    const end = parseIsoDate(dates.reduce((max, d) => (d > max ? d : max)));
    if (!start || !end) return undefined;

    return { start, end };
  }

  async getListByAccount(
    accountId: string
  ): Promise<AccountRecurringTransactionModel[]> {
    return this._indexDb.recurringTransactionsTable
      .where('accountId')
      .equals(accountId)
      .toArray();
  }

  async getAll(): Promise<AccountRecurringTransactionModel[]> {
    return this._indexDb.recurringTransactionsTable.toArray();
  }

  async deleteByAccount(accountId: string): Promise<void> {
    await this._indexDb.recurringTransactionsTable
      .where('accountId')
      .equals(accountId)
      .delete();
  }

  async delete(ids: string[]): Promise<void> {
    await this._indexDb.recurringTransactionsTable.bulkDelete(ids);
  }
}
