import { inject, Injectable } from '@angular/core';
import { AccountService } from '@services/account.service';
import { AccountModel, AccountType } from '@models/accountModel';
import { Result } from '@/types/result';
import { ApiResult, BdgHttpClient } from '@services/server/bdg-http-client.service';

@Injectable({ providedIn: 'root' })
export class AccountServiceServerImpl implements AccountService {

  private httpClient = inject<BdgHttpClient>(BdgHttpClient);

  async getList(): Promise<AccountModel[]> {

    var result = await this.httpClient.get<ApiResult<AccountModel[]>>('/api/Account/List');

    if (!result.succeeded) {
      throw new Error(`failed to create account ${name} with error : ${result.errorValue}`);
    }

    return result.successValue;
  }

  async create(
    name: string,
    columnsMappingId: string,
    accountType: AccountType,
    id?: string,
  ): Promise<Result<string>> {

    var result = await this.httpClient.post<ApiResult<string>>('/api/Account/AddOrUpdate', {
      id,
      name,
      columnsMappingId,
      accountType: accountType,
    });

    if (!result.succeeded) {
      if (id) return { success: false, error: 'id-exists' };
      if (result.errorValue === 'DuplicateAccountName') return { success: false, error: 'name-exists' };
      throw new Error(`failed to create account ${name} with error : ${result.errorValue}`);
    }

    return { success: true, value: result.successValue };
  }

  async getById(id: string): Promise<AccountModel> {
    var result = await this.httpClient.get<ApiResult<AccountModel>>(`/api/Account/${id}`);

    if (!result.succeeded) {
      throw new Error(`failed to get account ${id} with error : ${result.errorValue}`);
    }

    return this._normalize(result.successValue);
  }

  private _normalize(account: AccountModel): AccountModel {
    return { ...account, accountType: account.accountType ?? 'debit' };
  }

  async delete(id: string): Promise<void> {
    var result = await this.httpClient.delete<ApiResult<string>>(`/api/Account/${id}`);

    if (!result.succeeded) {
      throw new Error(`failed to delete account ${id} with error : ${result.errorValue}`);
    }
  }
}
