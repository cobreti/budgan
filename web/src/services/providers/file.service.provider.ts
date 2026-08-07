import { Provider } from '@angular/core';
import { environment } from '@/environments/environment';
import { FILE_SERVICE } from '@services/file.service';
import { FileServicePwaImpl } from '@services/pwa/file-pwa.service';
import { FileServiceServerImpl } from '@services/server/file-server.service';

export const FileServiceProvider: Provider = {
  provide: FILE_SERVICE,
  useClass: environment.buildType === 'server' ? FileServiceServerImpl : FileServicePwaImpl,
};
