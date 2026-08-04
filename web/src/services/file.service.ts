import { inject, Injectable, InjectionToken } from '@angular/core';
import { IndexdbService } from './indexdb.service';
import { fileModel } from '@models/fileModel';
import { ID_GENERATOR_SERVICE, IdGeneratorService } from './id-generator.service';
import { Result } from '@app-types/result';

export interface FileService {
  getList(): Promise<fileModel[]>;
  create(accountId: string, filename: string, content: string, insertionDate: Date): Promise<Result<string>>;
  getListByAccount(accountId: string): Promise<fileModel[]>;
  getById(id: string): Promise<fileModel>;
  delete(id: string): Promise<void>;
}

export const FILE_SERVICE = new InjectionToken<FileService>('FileService');

@Injectable({ providedIn: 'root' })
export class FileServiceImpl implements FileService {
  private readonly _indexDb = inject(IndexdbService);
  private readonly _idGenerator = inject<IdGeneratorService>(ID_GENERATOR_SERVICE);

  async getList(): Promise<fileModel[]> {
    return this._indexDb.filesTable.toArray();
  }

  async create(accountId: string, filename: string, content: string, insertionDate: Date): Promise<Result<string>> {
    const existing = await this._indexDb.filesTable
      .where('accountId').equals(accountId)
      .filter((f) => f.filename === filename)
      .first();
    if (existing) {
      return { success: false, error: 'file-already-imported' };
    }
    const id = this._idGenerator.generateId();
    await this._indexDb.filesTable.add({ id, accountId, filename, content, insertionDate });
    return { success: true, value: id };
  }

  async getListByAccount(accountId: string): Promise<fileModel[]> {
    return this._indexDb.filesTable.where('accountId').equals(accountId).toArray();
  }

  async getById(id: string): Promise<fileModel> {
    const entry = await this._indexDb.filesTable.get(id);
    if (!entry) {
      throw new Error('File not found');
    }
    return entry;
  }

  async delete(id: string): Promise<void> {
    await this._indexDb.filesTable.delete(id);
  }
}
