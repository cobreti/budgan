import type { BuildType } from '@/utils/build-type';

export const environment = {
  production: false,
  buildType: 'server' as BuildType,
  useServiceWorker: false,
  apiBaseUrl: 'http://localhost:3000/api' as string | null,
};
