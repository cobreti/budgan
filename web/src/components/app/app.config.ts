import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { environment } from '@/environments/environment';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideNativeDateAdapter } from '@angular/material/core';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

import { routes } from './app.routes';
import { ID_GENERATOR_SERVICE, IdGeneratorServiceImpl } from '@services/id-generator.service';
import { LOCALE_SERVICE, LocaleServiceImpl } from '@services/locale.service';
import { JOURNAL_SERVICE, JournalServiceImpl } from '@services/journal.service';
import { CSV_CONTENT_EXTRACTOR_SERVICE, CsvContentExtractorServiceImpl } from '@services/csv-content-extractor.service';
import { THEME_SERVICE, ThemeServiceImpl } from '@services/theme.service';
import { FileServiceProvider } from '@services/providers/file.service.provider';
import { BUDGAN_EXPORT_SERVICE, BudganExportServiceImpl } from '@services/budgan-export.service';
import { ACCOUNT_ANALYSIS_SERVICE, AccountAnalysisServiceImpl } from '@services/account-analysis.service';
import { API_MODE_SERVICE, ApiModeServiceImpl } from '@services/api-mode.service';
import { provideServiceWorker } from '@angular/service-worker';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { ColumnsMappingServiceProvider } from '@services/providers/columns-mapping.service.provider';
import { AccountServiceProvider } from '@services/providers/account.service.provider';
import { AccountTransactionServiceProvider } from '@services/providers/account-transaction.service.provider';
import { AccountRecurringTransactionServiceProvider } from '@services/providers/account-recurring-transaction.service.provider';
import { ImportServiceProvider } from '@services/providers/import.service.provider';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    provideAnimations(),
    provideNativeDateAdapter(),
    provideTranslateService({
      defaultLanguage: 'en',
    }),
    provideTranslateHttpLoader({
      prefix: '/assets/i18n/',
      suffix: '.json',
    }),
    { provide: ID_GENERATOR_SERVICE, useClass: IdGeneratorServiceImpl },
    { provide: LOCALE_SERVICE, useClass: LocaleServiceImpl },
    { provide: JOURNAL_SERVICE, useClass: JournalServiceImpl },
    ColumnsMappingServiceProvider,
    AccountServiceProvider,
    { provide: CSV_CONTENT_EXTRACTOR_SERVICE, useClass: CsvContentExtractorServiceImpl },
    { provide: THEME_SERVICE, useClass: ThemeServiceImpl },
    FileServiceProvider,
    AccountTransactionServiceProvider,
    AccountRecurringTransactionServiceProvider,
    ImportServiceProvider,
    { provide: BUDGAN_EXPORT_SERVICE, useClass: BudganExportServiceImpl },
    { provide: ACCOUNT_ANALYSIS_SERVICE, useClass: AccountAnalysisServiceImpl },
    { provide: API_MODE_SERVICE, useClass: ApiModeServiceImpl },
    provideCharts(withDefaultRegisterables()),
    provideServiceWorker('ngsw-worker.js', {
            enabled: environment.useServiceWorker,
            registrationStrategy: 'registerWhenStable:30000'
          }),
  ]
};
