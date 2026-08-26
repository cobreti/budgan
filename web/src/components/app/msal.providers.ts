import { EnvironmentProviders, inject, importProvidersFrom, Provider, provideAppInitializer } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { InteractionType, PublicClientApplication } from '@azure/msal-browser';
import { MsalBroadcastService, MsalGuard, MsalInterceptor, MsalModule, MsalService } from '@azure/msal-angular';
import { switchMap } from 'rxjs';
import { isServerBuild } from '@/utils/build-type';
import { environment } from '@/environments/environment';

export function msalProviders(): (Provider | EnvironmentProviders)[] {
  if (!isServerBuild() || !environment.auth) {
    return [];
  }
  const auth = environment.auth;
  const msalInstance = new PublicClientApplication({
    auth: {
      clientId: auth.clientId,
      authority: auth.authority,
      redirectUri: window.location.origin,
      postLogoutRedirectUri: window.location.origin,
    },
    cache: { cacheLocation: 'localStorage' },
  });

  return [
    importProvidersFrom(
      MsalModule.forRoot(
        msalInstance,
        { interactionType: InteractionType.Redirect },
        {
          interactionType: InteractionType.Redirect,
          protectedResourceMap: new Map([['/api/*', [auth.apiScope]]]),
        },
      ),
    ),
    { provide: HTTP_INTERCEPTORS, useClass: MsalInterceptor, multi: true },
    MsalGuard,
    MsalService,
    MsalBroadcastService,
    provideAppInitializer(() => {
      const msal = inject(MsalService);
      return msal.initialize().pipe(switchMap(() => msal.handleRedirectObservable()));
    }),
  ];
}
