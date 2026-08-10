import type { BuildType } from '@/utils/build-type';

export const environment = {
  production: true,
  buildType: 'pwa' as BuildType,
  useServiceWorker: true,
  apiBaseUrl: null as string | null,
};
