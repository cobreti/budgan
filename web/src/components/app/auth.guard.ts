import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { isServerBuild } from '@/utils/build-type';

export const authGuard: CanActivateFn = (route, state) => {
  if (!isServerBuild()) {
    return true;
  }
  return inject(MsalGuard).canActivate(route, state);
};
