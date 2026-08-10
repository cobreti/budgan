import type { BuildType } from '@/utils/build-type';

export const environment = {
  production: true,
  buildType: 'server' as BuildType,
  useServiceWorker: false,
  apiBaseUrl: '/api' as string | null,
};
