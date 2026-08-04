import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { JOURNAL_SERVICE, JournalService } from '@services/journal.service';
import { JournalModel } from '@models/journalModel';

@Component({
  selector: 'app-journal-list',
  templateUrl: './journal-list.component.html',
  styleUrl: './journal-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TranslatePipe],
})
export class JournalListComponent {
  private readonly _journalService = inject<JournalService>(JOURNAL_SERVICE);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);

  readonly journals = signal<JournalModel[]>([]);

  constructor() {
    this._load();
  }

  private async _load(): Promise<void> {
    this.journals.set(await this._journalService.getList());
  }

  open(id: string): void {
    this._router.navigate([this._locale.currentLocale(), 'journal', id]);
  }
}
