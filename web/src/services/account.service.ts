import { InjectionToken } from '@angular/core';
import { AccountModel, AccountType } from '@models/accountModel';
import { Result } from '@app-types/result';

export interface AccountService {
  getList(): Promise<AccountModel[]>;
  create(name: string, columnsMappingId: string, accountType: AccountType, id?: string): Promise<Result<string>>;
  getById(id: string): Promise<AccountModel>;
  delete(id: string): Promise<void>;
}

export const ACCOUNT_SERVICE = new InjectionToken<AccountService>('AccountService');


