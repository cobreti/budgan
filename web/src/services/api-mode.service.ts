import { Injectable, InjectionToken } from '@angular/core';
import { isPWABuild } from '@/utils/build-type';

export type ApiMode = 'local' | 'server';

export interface ApiModeService {
  readonly apiMode: ApiMode;
}

export const API_MODE_SERVICE = new InjectionToken<ApiModeService>('ApiModeService');

@Injectable({ providedIn: 'root' })
export class ApiModeServiceImpl implements ApiModeService {
  readonly apiMode: ApiMode = isPWABuild() ? 'local' : 'server';
}

