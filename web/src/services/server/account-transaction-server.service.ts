import { inject, Injectable, Signal, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import {
  AccountTransactionModel,
  AccountTransactionRecordType,
} from '@models/accountTransactionModel';
import { Result } from '@app-types/result';
import {
  ApiResult,
  BdgHttpClient,
} from '@services/server/bdg-http-client.service';
import {
  AccountTransactionService,
  TransactionPage,
  TransactionSort,
} from '@services/account-transaction.service';

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

interface PageResultDto {
  items: AccountTransactionDto[];
  totalCount: number;
}

@Injectable({ providedIn: 'root' })
export class AccountTransactionServiceServerImpl implements AccountTransactionService {
  private httpClient = inject<BdgHttpClient>(BdgHttpClient);
  private readonly _transactionsVersion = signal(0);

  readonly transactionsVersion: Signal<number> = this._transactionsVersion;

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

  async getList(): Promise<AccountTransactionModel[]> {
    throw new Error('getList is not supported in server mode');
  }

  async getCountByAccount(accountId: string): Promise<number> {
    const result = await this.httpClient.get<ApiResult<number>>(
      `/api/AccountTransaction/Account/${accountId}/Count`,
    );

    if (!result.succeeded) {
      throw new Error(
        `failed to get transaction count for account ${accountId} with error : ${result.errorValue}`,
      );
    }

    return result.successValue;
  }

  async getPageByAccount(
    accountId: string,
    page: number,
    pageSize: number,
    sort: TransactionSort,
  ): Promise<TransactionPage> {
    const query = `page=${page}&pageSize=${pageSize}&sortField=${encodeURIComponent(sort.field)}&sortDirection=${encodeURIComponent(sort.direction)}`;
    const result = await this.httpClient.get<ApiResult<PageResultDto>>(
      `/api/AccountTransaction/Account/${accountId}/Page?${query}`,
    );

    if (!result.succeeded) {
      throw new Error(
        `failed to get transaction page for account ${accountId} with error : ${result.errorValue}`,
      );
    }

    return {
      transactions: result.successValue.items.map((dto) => this._toModel(dto)),
      totalPages: Math.ceil(result.successValue.totalCount / pageSize),
      page,
    };
  }

  async create(
    fileId: string,
    accountId: string,
    cardNumber: string,
    dateInscriptionAsString: string,
    amount: number,
    description: string,
  ): Promise<Result<string>> {
    const result = await this.httpClient.post<ApiResult<string>>(
      '/api/AccountTransaction',
      {
        accountId,
        fileId,
        cardNumber,
        dateInscriptionAsString,
        amount,
        description,
      },
    );

    if (!result.succeeded) {
      return { success: false, error: 'duplicate-transaction' };
    }

    this._transactionsVersion.update((v) => v + 1);
    return { success: true, value: result.successValue };
  }

  async setSnapshot(
    accountId: string,
    dateAsString: string,
    amount: number,
  ): Promise<Result<string>> {
    const result = await this.httpClient.put<ApiResult<string>>(
      `/api/AccountTransaction/Account/${accountId}/Snapshot`,
      { dateAsString, amount },
    );

    if (!result.succeeded) {
      return { success: false, error: 'duplicate-transaction' };
    }

    this._transactionsVersion.update((v) => v + 1);
    return { success: true, value: result.successValue };
  }

  async getSnapshot(
    accountId: string,
  ): Promise<AccountTransactionModel | undefined> {
    const result = await this.httpClient.get<
      ApiResult<AccountTransactionDto | null>
    >(`/api/AccountTransaction/Account/${accountId}/Snapshot`);

    if (!result.succeeded || !result.successValue) {
      return undefined;
    }

    return this._toModel(result.successValue);
  }

  async deleteSnapshot(accountId: string): Promise<void> {
    await this.httpClient.delete<ApiResult<boolean>>(
      `/api/AccountTransaction/Account/${accountId}/Snapshot`,
    );
    this._transactionsVersion.update((v) => v + 1);
  }

  async getListByAccount(
    accountId: string,
  ): Promise<AccountTransactionModel[]> {
    const result = await this.httpClient.get<
      ApiResult<AccountTransactionDto[]>
    >(`/api/AccountTransaction/Account/${accountId}/List`);

    if (!result.succeeded) {
      throw new Error(
        `failed to list transactions for account ${accountId} with error : ${result.errorValue}`,
      );
    }

    return result.successValue.map((dto) => this._toModel(dto));
  }

  async getById(id: string): Promise<AccountTransactionModel> {
    try {
      const result = await this.httpClient.get<
        ApiResult<AccountTransactionDto>
      >(`/api/AccountTransaction/${id}`);

      if (!result.succeeded) {
        throw new Error('Account transaction not found');
      }

      return this._toModel(result.successValue);
    } catch (e) {
      if (e instanceof HttpErrorResponse && e.status === 404) {
        throw new Error('Account transaction not found');
      }
      throw e;
    }
  }

  async delete(id: string): Promise<void> {
    try {
      await this.httpClient.delete<ApiResult<string>>(
        `/api/AccountTransaction/${id}`,
      );
    } catch (e) {
      if (e instanceof HttpErrorResponse && e.status === 404) {
        return;
      }
      throw e;
    }
    this._transactionsVersion.update((v) => v + 1);
  }

  async recalculateBalances(_accountId: string): Promise<void> {
    this._transactionsVersion.update((v) => v + 1);
  }
}
