import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { MatCard } from '@angular/material/card';
import { JournalListComponent } from '@components/journal-list/journal-list.component';
import { Router } from '@angular/router';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-journals',
  templateUrl: 'journals.component.html',
  styleUrls: ['journals.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [PageMenuComponent, PageMenuButtonComponent, TranslatePipe, MatCard, JournalListComponent],
})
export class JournalsComponent {
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);

  onNewJournal(): void {
    this._router.navigate([this._locale.currentLocale(), 'journal', 'new']);
  }
}
