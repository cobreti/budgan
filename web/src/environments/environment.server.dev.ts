import type { BuildType } from '@/utils/build-type';
import type { AuthConfig } from '@app-types/auth-config';

export const environment = {
  production: false,
  buildType: 'server' as BuildType,
  useServiceWorker: false,
  apiBaseUrl: 'http://localhost:3000/api' as string | null,
  auth: {
    clientId: '<client-id>>',
    authority: 'https://<external tenant domain>.ciamlogin.com/<tenant-id>',
    apiScope: 'api://<client-id>/access_as_user',
  } as AuthConfig | null,
};
