import { Routes } from '@angular/router';
import { JournalsComponent } from '@views/journals/journals.component';
import { NewJournalComponent } from '@views/journals/new-journal/new-journal.component';
import { defaultLocaleGuard, localeGuard } from './locale.guard';
import { JournalDetailsComponent } from '@views/journals/journal-details/journal-details.component';
import { HomeComponent } from '@views/Home/home.component';
import { ColumnsMappingDetailsComponent } from '@views/columns-mapping/columns-mapping-details/columns-mapping-details.component';
import { NewColumnsMappingComponent } from '@views/columns-mapping/new-columns-mapping/new-columns-mapping.component';
import { NewAccountComponent } from '@views/accounts/new-account/new-account.component';
import { AccountHomeComponent } from '@views/accounts/account-home.component';
import { ImportFileComponent } from '@views/accounts/import-file/import-file.component';
import { SaveAccountComponent } from '@views/accounts/save-account/save-account.component';
import { AnalyzeAccountComponent } from '@views/accounts/analyze-account/analyze-account.component';
import { SaveComponent } from '@views/save/save.component';
import { LoadComponent } from '@views/load/load.component';
import { SamplesComponent } from '@views/samples/samples.component';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    canActivate: [defaultLocaleGuard],
    component: JournalsComponent,
  },
  {
    path: ':locale',
    canActivate: [localeGuard],
    children: [
      { path: '', component: HomeComponent },
      { path: 'journals', component: JournalsComponent },
      { path: 'journal/new', component: NewJournalComponent },
      { path: 'journal/:journalId', component: JournalDetailsComponent },
      { path: 'columns-mapping/new', component: NewColumnsMappingComponent },
      { path: 'columns-mapping/:columnsMappingId', component: ColumnsMappingDetailsComponent },
      { path: 'account/new', component: NewAccountComponent },
      { path: 'account/:accountId', component: AccountHomeComponent },
      { path: 'account/:accountId/import-file', component: ImportFileComponent },
      { path: 'account/:accountId/save', component: SaveAccountComponent },
      { path: 'account/:accountId/analyze', component: AnalyzeAccountComponent },
      { path: 'save', component: SaveComponent },
      { path: 'load', component: LoadComponent },
      { path: 'samples', component: SamplesComponent },
    ],
  },
  { path: '**', redirectTo: 'en' },
];
