import type { BuildType } from '@/utils/build-type';
import type { AuthConfig } from '@app-types/auth-config';

export const environment = {
  production: true,
  buildType: 'pwa' as BuildType,
  useServiceWorker: true,
  apiBaseUrl: null as string | null,
  auth: null as AuthConfig | null,
};
