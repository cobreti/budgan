import { environment } from '@/environments/environment';

export enum BuildType {
  pwa = 'pwa',
  server = 'server',
}

export function getCurrentBuildType(): BuildType {
  return environment.buildType;
}

export function isServerBuild(): boolean {
  return getCurrentBuildType() === BuildType.server;
}

export function isPWABuild(): boolean {
  return getCurrentBuildType() === BuildType.pwa;
}
