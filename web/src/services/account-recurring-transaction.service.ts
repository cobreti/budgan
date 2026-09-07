import { InjectionToken } from '@angular/core';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';
import { AccountTransactionModel } from '@models/accountTransactionModel';

export type RecurringTransactionsSpan = {
  start: Date;
  end: Date;
};

export type StructuredTransactionsByRecurringId = Record<
  string,
  AccountTransactionModel[]
>;
export interface AccountRecurringTransactionService {
  replaceForAccount(
    accountId: string,
    transactions: AccountRecurringTransactionModel[]
  ): Promise<void>;
  getRecurringTransactionsByAccount(
    accountId: string,
    startDate: Date,
    endDate: Date
  ): Promise<AccountTransactionModel[]>;
  getAllStructuredRecurringTransactions(
    startDate: Date,
    endDate: Date
  ): Promise<StructuredTransactionsByRecurringId>;
  getStructuredRecurringTransactionsByAccount(
    accountId: string,
    startDate: Date,
    endDate: Date
  ): Promise<StructuredTransactionsByRecurringId>;
  getRecurringTransactionsSpan(
    accountId: string
  ): Promise<RecurringTransactionsSpan | undefined>;
  getListByAccount(
    accountId: string
  ): Promise<AccountRecurringTransactionModel[]>;
  getAll(): Promise<AccountRecurringTransactionModel[]>;
  deleteByAccount(accountId: string): Promise<void>;
  delete(ids: string[]): Promise<void>;
}

export const ACCOUNT_RECURRING_TRANSACTION_SERVICE =
  new InjectionToken<AccountRecurringTransactionService>(
    'AccountRecurringTransactionService'
  );
