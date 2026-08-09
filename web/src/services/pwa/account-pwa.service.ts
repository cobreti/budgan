import { inject, Injectable } from '@angular/core';
import Dexie from 'dexie';
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
    id?: string,
  ): Promise<Result<string>> {
    const query = this._indexDb.accountsTable.where('name').equals(name);
    const nameConflict = id ? await query.filter((a) => a.id !== id).count() : await query.count();
    if (nameConflict > 0) return { success: false, error: 'name-exists' };

    const finalId = id ?? this._idGenerator.generateId();
    try {
      await this._indexDb.accountsTable.add({ id: finalId, name, columnsMappingId, accountType });
      return { success: true, value: finalId };
    } catch (e) {
      if (id && e instanceof Dexie.ConstraintError) return { success: false, error: 'id-exists' };
      throw e;
    }
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
