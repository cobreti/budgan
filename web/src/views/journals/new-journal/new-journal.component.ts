import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { MatError } from '@angular/material/form-field';
import { JOURNAL_SERVICE, JournalService } from '@services/journal.service';

@Component({
  selector: 'app-new-journal',
  templateUrl: './new-journal.component.html',
  styleUrl: './new-journal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, MatFormField, MatLabel, MatError, MatInput, MatButton, TranslatePipe],
})
export class NewJournalComponent {
  private readonly _journalService = inject<JournalService>(JOURNAL_SERVICE);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);

  readonly nameControl = new FormControl('', { nonNullable: true, validators: [Validators.required] });

  async onCreate(): Promise<void> {
    if (this.nameControl.invalid) return;
    const result = await this._journalService.create(this.nameControl.value);
    if (!result.success) {
      this.nameControl.setErrors({ nameExists: true });
      return;
    }
    await this._router.navigate([this._locale.currentLocale(), 'journal', result.value]);
  }

  onCancel(): void {
    this._router.navigate([this._locale.currentLocale()]);
  }
}
