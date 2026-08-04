import { inject, Injectable, InjectionToken } from '@angular/core';
import { IndexdbService } from './indexdb.service';
import { JournalModel } from '@models/journalModel';
import { ID_GENERATOR_SERVICE, IdGeneratorService } from './id-generator.service';
import { Result } from '@app-types/result';

export interface JournalService {
  getList(): Promise<JournalModel[]>;
  create(name: string): Promise<Result<string>>;
  getById(id: string): Promise<JournalModel>;
  delete(id: string): Promise<void>;
}

export const JOURNAL_SERVICE = new InjectionToken<JournalService>('JournalService');

@Injectable({ providedIn: 'root' })
export class JournalServiceImpl implements JournalService {
  private readonly _indexDb = inject(IndexdbService);
  private readonly _idGenerator = inject<IdGeneratorService>(ID_GENERATOR_SERVICE);

  async getList(): Promise<JournalModel[]> {
    return this._indexDb.workspaceTable.toArray();
  }

  async create(name: string): Promise<Result<string>> {
    const existing = await this._indexDb.workspaceTable.where('name').equals(name).count();
    if (existing > 0) return { success: false, error: 'name-exists' };

    const id = this._idGenerator.generateId();
    await this._indexDb.workspaceTable.add({ id, name });
    return { success: true, value: id };
  }

  async getById(id: string): Promise<JournalModel> {
    const entry = await this._indexDb.workspaceTable.get(id);
    if (!entry) {
      throw new Error('Workspace not found');
    }
    return entry;
  }

  async delete(id: string): Promise<void> {
    await this._indexDb.workspaceTable.delete(id);
  }
}
