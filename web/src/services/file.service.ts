import { InjectionToken } from '@angular/core';
import { fileModel } from '@models/fileModel';
import { Result } from '@app-types/result';

export interface FileService {
  getList(): Promise<fileModel[]>;
  create(accountId: string, filename: string, content: string, insertionDate: Date): Promise<Result<string>>;
  getListByAccount(accountId: string): Promise<fileModel[]>;
  getById(id: string): Promise<fileModel>;
  delete(id: string): Promise<void>;
}

export const FILE_SERVICE = new InjectionToken<FileService>('FileService');
