import { inject, Injectable } from '@angular/core';
import { AccountService } from '@services/account.service';
import { ID_GENERATOR_SERVICE, IdGeneratorService } from '@services/id-generator.service';
import { IndexdbService } from '@services/indexdb.service';
import { AccountModel, AccountType } from '@models/accountModel';
import { Result } from '@/types/result';

@Injectable({ providedIn: 'root' })
export class AccountServicePwaImpl implements AccountService {
  private readonly _indexDb = inject(IndexdbService);
  private readonly _idGenerator = inject<IdGeneratorService>(ID_GENERATOR_SERVICE);

  async getList(): Promise<AccountModel[]> {
    const entries = await this._indexDb.accountsTable.toArray();
    return entries.map((entry) => this._normalize(entry));
  }

  async create(
    name: string,
    columnsMappingId: string,
    accountType: AccountType,
  ): Promise<Result<string>> {
    const existing = await this._indexDb.accountsTable.where('name').equals(name).count();
    if (existing > 0) return { success: false, error: 'name-exists' };

    const id = this._idGenerator.generateId();
    await this._indexDb.accountsTable.add({ id, name, columnsMappingId, accountType });
    return { success: true, value: id };
  }

  async getById(id: string): Promise<AccountModel> {
    const entry = await this._indexDb.accountsTable.get(id);
    if (!entry) {
      throw new Error('Account not found');
    }
    return this._normalize(entry);
  }

  private _normalize(account: AccountModel): AccountModel {
    return { ...account, accountType: account.accountType ?? 'debit' };
  }

  async delete(id: string): Promise<void> {
    await this._indexDb.transaction(
      'rw',
      [
        this._indexDb.accountsTable,
        this._indexDb.filesTable,
        this._indexDb.accountTransactionsTable,
        this._indexDb.recurringTransactionsTable,
      ],
      async () => {
        await this._indexDb.accountTransactionsTable.where('accountId').equals(id).delete();
        await this._indexDb.recurringTransactionsTable.where('accountId').equals(id).delete();
        await this._indexDb.filesTable.where('accountId').equals(id).delete();
        await this._indexDb.accountsTable.delete(id);
      },
    );
  }
}
