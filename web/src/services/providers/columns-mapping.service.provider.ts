import { Provider } from '@angular/core';
import { COLUMNS_MAPPING_SERVICE, } from '@services/columns-mapping.service';
import { ColumnsMappingServicePwaImpl } from '@services/pwa/columns-mapping-pwa.service';
import { environment } from '@/environments/environment';
import { ColumnsMappingServiceServerImpl } from '@services/server/columns-mapping-server.service';

export const ColumnsMappingServiceProvider: Provider = {
  provide: COLUMNS_MAPPING_SERVICE,
  useClass: environment.buildType === 'server' ? ColumnsMappingServiceServerImpl : ColumnsMappingServicePwaImpl
};
