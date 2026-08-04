import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  WritableSignal,
} from '@angular/core';
import { JOURNAL_SERVICE, JournalService } from '@services/journal.service';
import { ActivatedRoute, Router } from '@angular/router';
import { JournalModel } from '@models/journalModel';
import { TranslatePipe } from '@ngx-translate/core';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';

@Component({
  selector: 'app-journal-details',
  templateUrl: './journal-details.component.html',
  styleUrl: './journal-details.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TranslatePipe, PageMenuComponent, PageMenuButtonComponent],
})
export class JournalDetailsComponent implements OnInit {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _journalService = inject<JournalService>(JOURNAL_SERVICE);

  readonly journal: WritableSignal<JournalModel | undefined> = signal(undefined);

  async onDelete(): Promise<void> {
    const journal = this.journal();

    if (!journal)
    {
      throw new Error('Journal is undefined');
    }

    await this._journalService.delete(journal.id);
    await this._router.navigate([this._locale.currentLocale(), 'journals']);
  }

  async ngOnInit(): Promise<void> {
    const id = this._route.snapshot.params['journalId'] ?? undefined;
    const entry = await this._journalService.getById(id);
    if (!entry) {
      console.error('Failed to load journal with ID:', id);
      return;
    }
    this.journal.set(entry);
  }
}
