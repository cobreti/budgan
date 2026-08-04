import { inject, Injectable } from '@angular/core';
import { ColumnsMappingService } from '@services/columns-mapping.service';
import { ColumnsMapping } from '@models/columnsMappingModel';
import { Result } from '@/types/result';
import { BdgHttpClient } from '@services/server/bdg-http-client.service';

@Injectable({ providedIn: 'root' })
export class ColumnsMappingServiceServerImpl implements ColumnsMappingService {

  private httpClient = inject<BdgHttpClient>(BdgHttpClient);

  async getList(): Promise<ColumnsMapping[]> {
    var result = await this.httpClient.get<ColumnsMapping[]>('/api/ColumnsMapping/list');

    return result;
  }

  async save(mapping: ColumnsMapping): Promise<Result<ColumnsMapping>> {
    var resultId = await this.httpClient.post<string>('/api/ColumnsMapping/AddOrUpdate', mapping);

    var newMapping: ColumnsMapping = {
      id: resultId,
      ...mapping
    }

    return { success: true, value: newMapping };
  }

  async getById(id: string): Promise<ColumnsMapping> {
    return await this.httpClient.get<ColumnsMapping>(`/api/ColumnsMapping/${id}`, );
  }

  async delete(id: string): Promise<void> {
    await this.httpClient.delete(`/api/ColumnsMapping/${id}`);
  }
}
