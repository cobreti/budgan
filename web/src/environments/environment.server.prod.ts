import type { BuildType } from '@/utils/build-type';
import type { AuthConfig } from '@app-types/auth-config';

export const environment = {
  production: true,
  buildType: 'server' as BuildType,
  useServiceWorker: false,
  apiBaseUrl: '/api' as string | null,
  auth: {
    clientId: '<client-id>',
    authority: 'https://<tenant-subdomain>.ciamlogin.com/<tenant-id>/',
    apiScope: 'api://<client-id>/access_as_user',
  } as AuthConfig | null,
};
