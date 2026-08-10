import { Provider } from '@angular/core';
import { isServerBuild } from '@/utils/build-type';
import { FILE_SERVICE } from '@services/file.service';
import { FileServicePwaImpl } from '@services/pwa/file-pwa.service';
import { FileServiceServerImpl } from '@services/server/file-server.service';

export const FileServiceProvider: Provider = {
  provide: FILE_SERVICE,
  useClass: isServerBuild() ? FileServiceServerImpl : FileServicePwaImpl,
};
