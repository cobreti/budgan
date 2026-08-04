import { InjectionToken } from '@angular/core';
import { ColumnsMapping } from '@models/columnsMappingModel';
import { Result } from '@app-types/result';

export interface ColumnsMappingService {
  getList(): Promise<ColumnsMapping[]>;
  save(mapping: ColumnsMapping): Promise<Result<ColumnsMapping>>;
  getById(id: string): Promise<ColumnsMapping>;
  delete(id: string): Promise<void>;
}

export const COLUMNS_MAPPING_SERVICE = new InjectionToken<ColumnsMappingService>(
  'ColumnsMappingService',
);
