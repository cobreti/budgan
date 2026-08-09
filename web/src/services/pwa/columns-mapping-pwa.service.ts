import { inject, Injectable } from '@angular/core';
import Dexie from 'dexie';
import { ColumnsMappingService } from '@services/columns-mapping.service';
import { IndexdbService } from '@services/indexdb.service';
import { ID_GENERATOR_SERVICE, IdGeneratorService } from '@services/id-generator.service';
import { ColumnsMapping } from '@models/columnsMappingModel';
import { Result } from '@/types/result';

@Injectable({ providedIn: 'root' })
export class ColumnsMappingServicePwaImpl implements ColumnsMappingService {
  private readonly _indexDb = inject(IndexdbService);
  private readonly _idGenerator = inject<IdGeneratorService>(ID_GENERATOR_SERVICE);

  async getList(): Promise<ColumnsMapping[]> {
    return this._indexDb.columnsMappingTable.toArray();
  }

  async save(mapping: ColumnsMapping): Promise<Result<ColumnsMapping>> {
    if (mapping.id) {
      const conflict = await this._indexDb.columnsMappingTable
        .where('name')
        .equals(mapping.name)
        .filter((r) => r.id !== mapping.id)
        .count();
      if (conflict > 0) return { success: false, error: 'name-exists' };
      try {
        await this._indexDb.columnsMappingTable.add(mapping);
        return { success: true, value: mapping };
      } catch (e) {
        if (e instanceof Dexie.ConstraintError) return { success: false, error: 'id-exists' };
        throw e;
      }
    } else {
      const existing = await this._indexDb.columnsMappingTable
        .where('name')
        .equals(mapping.name)
        .count();
      if (existing > 0) return { success: false, error: 'name-exists' };
      const id = this._idGenerator.generateId();
      const record = { ...mapping, id };
      await this._indexDb.columnsMappingTable.add(record);
      return { success: true, value: record };
    }
  }

  async getById(id: string): Promise<ColumnsMapping> {
    const entry = await this._indexDb.columnsMappingTable.get(id);
    if (!entry) {
      throw new Error('Columns mapping not found');
    }
    return entry;
  }

  async delete(id: string): Promise<void> {
    await this._indexDb.columnsMappingTable.delete(id);
  }
}

