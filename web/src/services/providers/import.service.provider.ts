import { Provider } from '@angular/core';
import { environment } from '@/environments/environment';
import { IMPORT_SERVICE } from '@services/import.service';
import { ImportServicePwaImpl } from '@services/pwa/import-pwa.service';
import { ImportServiceServerImpl } from '@services/server/import-server.service';

export const ImportServiceProvider: Provider = {
  provide: IMPORT_SERVICE,
  useClass: environment.buildType === 'server' ? ImportServiceServerImpl : ImportServicePwaImpl,
};
