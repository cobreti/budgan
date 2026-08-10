import type { BuildType } from '@/utils/build-type';

export const environment = {
  production: false,
  buildType: 'pwa' as BuildType,
  useServiceWorker: false,
  apiBaseUrl: null as string | null,
};
