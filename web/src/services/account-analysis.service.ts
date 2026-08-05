import { inject, Injectable, InjectionToken } from '@angular/core';
import moment from 'moment';
import { ACCOUNT_TRANSACTION_SERVICE, AccountTransactionService } from './account-transaction.service';
import { AccountTransactionRecordType } from '@models/accountTransactionModel';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';
import {
  ACCOUNT_RECURRING_TRANSACTION_SERVICE,
  AccountRecurringTransactionService,
} from './account-recurring-transaction.service';
import { Result } from '@app-types/result';

export interface AccountAnalysisService {
  analyzeAccount(accountId: string): Promise<Result<AccountRecurringTransactionModel[]>>;
  applyAnalysis(
    accountId: string,
    transactions: AccountRecurringTransactionModel[],
  ): Promise<Result<void>>;
  getRecurringTransactions(accountId: string): Promise<AccountRecurringTransactionModel[]>;
  getAll(): Promise<AccountRecurringTransactionModel[]>;
  deleteByAccount(accountId: string): Promise<void>;
  delete(ids: string[]): Promise<void>;
}

export const ACCOUNT_ANALYSIS_SERVICE = new InjectionToken<AccountAnalysisService>('AccountAnalysisService');

@Injectable({ providedIn: 'root' })
export class AccountAnalysisServiceImpl implements AccountAnalysisService {
  private readonly _transactionService = inject<AccountTransactionService>(ACCOUNT_TRANSACTION_SERVICE);
  private readonly _recurringTransactionService = inject<AccountRecurringTransactionService>(
    ACCOUNT_RECURRING_TRANSACTION_SERVICE,
  );

  async analyzeAccount(accountId: string): Promise<Result<AccountRecurringTransactionModel[]>> {
    try {
      const allTransactions = await this._transactionService.getListByAccount(accountId);

      const normal = allTransactions.filter(
        (t) => t.recordType === AccountTransactionRecordType.normal,
      );

      const groups = new Map<string, typeof normal>();
      for (const t of normal) {
        const existing = groups.get(t.recurringId) ?? [];
        existing.push(t);
        groups.set(t.recurringId, existing);
      }

      const results: AccountRecurringTransactionModel[] = [];
      for (const [recurringId, transactions] of groups) {
        if (transactions.length < 2) continue;

        const sorted = [...transactions].sort((a, b) =>
          a.dateInscriptionAsString.localeCompare(b.dateInscriptionAsString),
        );

        const intervals: number[] = [];
        for (let i = 1; i < sorted.length; i++) {
          const prev = moment(sorted[i - 1].dateInscriptionAsString);
          const curr = moment(sorted[i].dateInscriptionAsString);
          const diffDays = curr.diff(prev, 'days');
          if (diffDays > 0) intervals.push(diffDays);
        }

        if (intervals.length === 0) continue;

        const transactionCount = transactions.length;
        const description = sorted[0].description;
        const averageAmount = transactions.reduce((sum, t) => sum + t.amount, 0) / transactionCount;
        const firstOccurrenceDate = sorted[0].dateInscriptionAsString;
        const lastOccurrenceDate = sorted[sorted.length - 1].dateInscriptionAsString;

        results.push({
          id: recurringId,
          accountId,
          periodInDays: median(intervals),
          transactionCount,
          description,
          averageAmount,
          firstOccurrenceDate,
          lastOccurrenceDate,
        });
      }

      return { success: true, value: results };
    } catch (e) {
      return { success: false, error: e instanceof Error ? e.message : 'analysis-failed' };
    }
  }

  async applyAnalysis(
    accountId: string,
    transactions: AccountRecurringTransactionModel[],
  ): Promise<Result<void>> {
    try {
      await this._recurringTransactionService.replaceForAccount(accountId, transactions);

      return { success: true, value: undefined };
    } catch (e) {
      return { success: false, error: e instanceof Error ? e.message : 'apply-analysis-failed' };
    }
  }

  getRecurringTransactions(accountId: string): Promise<AccountRecurringTransactionModel[]> {
    return this._recurringTransactionService.getListByAccount(accountId);
  }

  getAll(): Promise<AccountRecurringTransactionModel[]> {
    return this._recurringTransactionService.getAll();
  }

  async deleteByAccount(accountId: string): Promise<void> {
    await this._recurringTransactionService.deleteByAccount(accountId);
  }

  async delete(ids: string[]): Promise<void> {
    await this._recurringTransactionService.delete(ids);
  }
}

function median(values: number[]): number {
  const sorted = [...values].sort((a, b) => a - b);
  const mid = Math.floor(sorted.length / 2);
  return sorted.length % 2 === 0 ? (sorted[mid - 1] + sorted[mid]) / 2 : sorted[mid];
}
