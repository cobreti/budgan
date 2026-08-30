import type { BuildType } from '@/utils/build-type';
import type { AuthConfig } from '@app-types/auth-config';

export const environment = {
  production: false,
  buildType: 'pwa' as BuildType,
  useServiceWorker: false,
  apiBaseUrl: null as string | null,
  auth: null as AuthConfig | null,
};
