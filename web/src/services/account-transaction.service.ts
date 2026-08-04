import { InjectionToken, Signal } from '@angular/core';
import { AccountTransactionModel } from '@models/accountTransactionModel';
import { Result } from '@app-types/result';

export type TransactionPage = {
  transactions: AccountTransactionModel[];
  totalPages: number;
  page: number;
};

export type RecurringTransactionsSpan = {
  start: Date;
  end: Date;
};

export type TransactionSortField = 'cardNumber' | 'dateInscription' | 'description' | 'amount';

export type TransactionSort = {
  field: TransactionSortField;
  direction: 'asc' | 'desc';
};

export interface AccountTransactionService {
  readonly transactionsVersion: Signal<number>;
  getList(): Promise<AccountTransactionModel[]>;
  getCountByAccount(accountId: string): Promise<number>;
  getPageByAccount(
    accountId: string,
    page: number,
    pageSize: number,
    sort: TransactionSort,
  ): Promise<TransactionPage>;
  create(
    fileId: string,
    accountId: string,
    cardNumber: string,
    dateInscriptionAsString: string,
    amount: number,
    description: string,
  ): Promise<Result<string>>;
  setSnapshot(accountId: string, dateAsString: string, amount: number): Promise<Result<string>>;
  getSnapshot(accountId: string): Promise<AccountTransactionModel | undefined>;
  deleteSnapshot(accountId: string): Promise<void>;
  getListByAccount(accountId: string): Promise<AccountTransactionModel[]>;
  getRecurringTransactionsByAccount(
    accountId: string,
    startDate: Date,
    endDate: Date,
  ): Promise<AccountTransactionModel[]>;
  getRecurringTransactionsSpan(accountId: string): Promise<RecurringTransactionsSpan | undefined>;
  getById(id: string): Promise<AccountTransactionModel>;
  delete(id: string): Promise<void>;
  recalculateBalances(accountId: string): Promise<void>;
}

export const ACCOUNT_TRANSACTION_SERVICE = new InjectionToken<AccountTransactionService>(
  'AccountTransactionService',
);
