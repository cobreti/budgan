import { inject, Injectable } from '@angular/core';
import { fileModel } from '@models/fileModel';
import { Result } from '@app-types/result';
import { ApiResult, BdgHttpClient } from '@services/server/bdg-http-client.service';
import { FileService } from '@services/file.service';
import { formatIsoDate, parseIsoDate } from '@/utils/date';

interface FileDto {
  id: string;
  accountId: string;
  content: string;
  filename: string;
  insertionDate: string;
}

@Injectable({ providedIn: 'root' })
export class FileServiceServerImpl implements FileService {
  private httpClient = inject<BdgHttpClient>(BdgHttpClient);

  private _toModel(dto: FileDto): fileModel {
    return {
      id: dto.id,
      accountId: dto.accountId,
      content: dto.content,
      filename: dto.filename,
      insertionDate: parseIsoDate(dto.insertionDate)!,
    };
  }

  async getList(): Promise<fileModel[]> {
    throw new Error('getList is not supported in server mode');
  }

  async create(accountId: string, filename: string, content: string, insertionDate: Date): Promise<Result<string>> {
    const result = await this.httpClient.post<ApiResult<string>>('/api/TransactionsFile/Save', {
      accountId,
      filename,
      content,
      insertionDate: formatIsoDate(insertionDate),
    });

    if (!result.succeeded) {
      return { success: false, error: 'file-already-imported' };
    }

    return { success: true, value: result.successValue };
  }

  async getListByAccount(accountId: string): Promise<fileModel[]> {
    const result = await this.httpClient.get<FileDto[]>(`/api/TransactionsFile/ListByAccount/${accountId}`);

    return result.map((dto) => this._toModel(dto));
  }

  async getById(_id: string): Promise<fileModel> {
    throw new Error('getById is not supported in server mode');
  }

  async delete(_id: string): Promise<void> {
    throw new Error('delete is not supported in server mode');
  }
}
