import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { LOCALE_SERVICE } from '@services/locale.service';

export const defaultLocaleGuard: CanActivateFn = () => {
  const locale = inject(LOCALE_SERVICE).detectBrowserLocale();
  return inject(Router).createUrlTree([locale]);
};

export const localeGuard: CanActivateFn = (route) => {
  const locale = route.paramMap.get('locale');
  const localeService = inject(LOCALE_SERVICE);

  if (!locale || !(localeService.supportedLocales as readonly string[]).includes(locale)) {
    return inject(Router).createUrlTree(['en']);
  }

  localeService.setLocale(locale);
  inject(TranslateService).use(locale);
  return true;
};
