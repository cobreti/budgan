import type { BuildType } from '@/utils/build-type';
import type { AuthConfig } from '@app-types/auth-config';

export const environment = {
  production: false,
  buildType: 'server' as BuildType,
  useServiceWorker: false,
  apiBaseUrl: 'http://localhost:3000/api' as string | null,
  auth: {
    clientId: '00000000-0000-0000-0000-000000000000',
    authority: 'https://login.microsoftonline.com/common',
    apiScope: 'api://00000000-0000-0000-0000-000000000000/access_as_user',
  } as AuthConfig | null,
};
