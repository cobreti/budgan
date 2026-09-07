import { inject, Injectable } from '@angular/core';
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
import { ApiResult, BdgHttpClient } from './bdg-http-client.service';

interface AccountTransactionDto {
  id: string;
  accountId: string;
  fileId?: string | null;
  cardNumber: string;
  dateInscriptionAsString: string;
  amount: number;
  balance?: number | null;
  balanceDateOffset?: number | null;
  description: string;
  uniqueKey: string;
  recurringId: string;
  recordType: string;
}

interface RecurringTransactionsSpanDto {
  start: string;
  end: string;
}

interface AccountRecurringTransactionItemDto {
  id: string;
  periodInDays: number;
  transactionCount: number;
  description: string;
  averageAmount: number;
  firstOccurrenceDate: string;
  lastOccurrenceDate: string;
}

interface AccountRecurringTransactionListItemDto extends AccountRecurringTransactionItemDto {
  accountId: string;
}

@Injectable({ providedIn: 'root' })
export class AccountRecurringTransactionServiceServerImpl implements AccountRecurringTransactionService {
  private readonly _httpClient = inject(BdgHttpClient);

  private _toModel(dto: AccountTransactionDto): AccountTransactionModel {
    return {
      id: dto.id,
      uniqueKey: dto.uniqueKey,
      recurringId: dto.recurringId,
      fileId: dto.fileId ?? '',
      accountId: dto.accountId,
      cardNumber: dto.cardNumber,
      dateInscriptionAsString: dto.dateInscriptionAsString,
      amount: dto.amount,
      balance: dto.balance ?? undefined,
      balanceDateOffset: dto.balanceDateOffset ?? undefined,
      description: dto.description,
      recordType:
        dto.recordType === 'snapshot'
          ? AccountTransactionRecordType.snapshot
          : AccountTransactionRecordType.normal,
    };
  }

  async replaceForAccount(
    _accountId: string,
    _transactions: AccountRecurringTransactionModel[]
  ): Promise<void> {
    await this._httpClient.put<void>(
      `/api/AccountRecurringTransaction/account/${_accountId}`,
      _transactions
    );
  }

  async getRecurringTransactionsByAccount(
    accountId: string,
    startDate: Date,
    endDate: Date
  ): Promise<AccountTransactionModel[]> {
    const query = `startDate=${formatIsoDate(startDate)}&endDate=${formatIsoDate(endDate)}`;
    const result = await this._httpClient.get<
      ApiResult<AccountTransactionDto[]>
    >(
      `/api/AccountRecurringTransaction/Account/${accountId}/Transactions?${query}`
    );

    if (!result.succeeded) {
      throw new Error(
        `failed to get recurring transactions for account ${accountId} with error : ${result.errorValue}`
      );
    }

    return result.successValue.map((dto) => this._toModel(dto));
  }

  async getAllStructuredRecurringTransactions(
    startDate: Date,
    endDate: Date
  ): Promise<StructuredTransactionsByRecurringId> {
    const query = `startDate=${formatIsoDate(startDate)}&endDate=${formatIsoDate(endDate)}`;
    const result = await this._httpClient.get<
      ApiResult<AccountTransactionDto[]>
    >(`/api/AccountRecurringTransaction/Account/Transactions?${query}`);

    const structured: Record<string, AccountTransactionModel[]> = {};
    for (const dto of result.successValue) {
      const model = this._toModel(dto);
      if (!structured[model.recurringId]) {
        structured[model.recurringId] = [];
      }
      structured[model.recurringId].push(model);
    }

    return structured;
  }

  async getStructuredRecurringTransactionsByAccount(
    accountId: string,
    startDate: Date,
    endDate: Date
  ): Promise<StructuredTransactionsByRecurringId> {
    const query = `startDate=${formatIsoDate(startDate)}&endDate=${formatIsoDate(endDate)}`;
    const result = await this._httpClient.get<
      ApiResult<AccountTransactionDto[]>
    >(
      `/api/AccountRecurringTransaction/Account/${accountId}/Transactions?${query}`
    );

    if (!result.succeeded) {
      throw new Error(
        `failed to get recurring transactions for account ${accountId} with error : ${result.errorValue}`
      );
    }

    const structured: Record<string, AccountTransactionModel[]> = {};
    for (const dto of result.successValue) {
      const model = this._toModel(dto);
      if (!structured[model.recurringId]) {
        structured[model.recurringId] = [];
      }
      structured[model.recurringId].push(model);
    }

    return structured;
  }

  async getRecurringTransactionsSpan(
    accountId: string
  ): Promise<RecurringTransactionsSpan | undefined> {
    const result = await this._httpClient.get<
      ApiResult<RecurringTransactionsSpanDto | null>
    >(`/api/AccountRecurringTransaction/Account/${accountId}/Span`);

    if (!result.succeeded || !result.successValue) {
      return undefined;
    }

    const start = parseIsoDate(result.successValue.start);
    const end = parseIsoDate(result.successValue.end);
    if (!start || !end) {
      return undefined;
    }

    return { start, end };
  }

  async getListByAccount(
    accountId: string
  ): Promise<AccountRecurringTransactionModel[]> {
    const result = await this._httpClient.get<
      ApiResult<AccountRecurringTransactionItemDto[]>
    >(`/api/AccountRecurringTransaction/Account/${accountId}/List`);

    if (!result.succeeded) {
      throw new Error(
        `failed to list recurring transactions for account ${accountId} with error : ${result.errorValue}`
      );
    }

    return result.successValue.map((dto) => ({
      id: dto.id,
      accountId,
      periodInDays: dto.periodInDays,
      transactionCount: dto.transactionCount,
      description: dto.description,
      averageAmount: dto.averageAmount,
      firstOccurrenceDate: dto.firstOccurrenceDate,
      lastOccurrenceDate: dto.lastOccurrenceDate,
    }));
  }

  async getAll(): Promise<AccountRecurringTransactionModel[]> {
    const result = await this._httpClient.get<
      ApiResult<AccountRecurringTransactionListItemDto[]>
    >('/api/AccountRecurringTransaction/List');

    if (!result.succeeded) {
      throw new Error(
        `failed to list all recurring transactions with error : ${result.errorValue}`
      );
    }

    return result.successValue.map((dto) => ({
      id: dto.id,
      accountId: dto.accountId,
      periodInDays: dto.periodInDays,
      transactionCount: dto.transactionCount,
      description: dto.description,
      averageAmount: dto.averageAmount,
      firstOccurrenceDate: dto.firstOccurrenceDate,
      lastOccurrenceDate: dto.lastOccurrenceDate,
    }));
  }

  async deleteByAccount(accountId: string): Promise<void> {
    await this._httpClient.delete<ApiResult<boolean>>(
      `/api/AccountRecurringTransaction/Account/${accountId}`
    );
  }

  async delete(_ids: string[]): Promise<void> {
    throw new Error('delete is not supported in server mode');
  }
}
