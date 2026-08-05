import {
  ChangeDetectionStrategy,
  Component,
  effect,
  inject,
  input,
  signal,
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard, MatCardContent, MatCardTitle } from '@angular/material/card';
import { DateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import {
  MatFormField,
  MatLabel,
  MatSuffix,
} from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { TranslatePipe } from '@ngx-translate/core';
import {
  ACCOUNT_TRANSACTION_SERVICE,
  AccountTransactionService,
} from '@services/account-transaction.service';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '@components/confirm-dialog/confirm-dialog.component';
import { formatIsoDate, parseIsoDate } from '@/utils/date';

@Component({
  selector: 'app-account-snapshot',
  templateUrl: './account-snapshot.component.html',
  styleUrl: './account-snapshot.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatCardTitle,
    MatCardContent,
    MatFormField,
    MatLabel,
    MatSuffix,
    MatInput,
    MatDatepickerModule,
    MatButton,
    TranslatePipe,
  ],
})
export class AccountSnapshotComponent {
  private readonly _transactionService = inject<AccountTransactionService>(
    ACCOUNT_TRANSACTION_SERVICE,
  );
  private readonly _dialog = inject(MatDialog);
  private readonly _dateAdapter = inject<DateAdapter<Date>>(DateAdapter);
  private readonly _localeService = inject<LocaleService>(LOCALE_SERVICE);

  readonly accountId = input.required<string>();

  readonly form = new FormGroup({
    date: new FormControl<Date | null>(null, [Validators.required]),
    amount: new FormControl<number | null>(null, [Validators.required]),
  });

  readonly hasSnapshot = signal(false);

  constructor() {
    effect(() => {
      const id = this.accountId();
      this._loadSnapshot(id);
    });

    effect(() => {
      this._dateAdapter.setLocale(this._localeService.currentLocale());
    });
  }

  private async _loadSnapshot(accountId: string): Promise<void> {
    const snapshot = await this._transactionService.getSnapshot(accountId);
    if (snapshot) {
      this.form.patchValue({
        date: parseIsoDate(snapshot.dateInscriptionAsString),
        amount: snapshot.amount,
      });
      this.hasSnapshot.set(true);
    } else {
      this.form.reset();
      this.hasSnapshot.set(false);
    }
  }

  async onSave(): Promise<void> {
    if (this.form.invalid) return;
    const { date, amount } = this.form.getRawValue();
    if (date === null || amount === null) return;

    const result = await this._transactionService.setSnapshot(
      this.accountId(),
      formatIsoDate(date),
      amount,
    );
    if (result.success) {
      this.hasSnapshot.set(true);
      this.form.markAsPristine();
    }
  }

  async onDelete(): Promise<void> {
    if (!this.hasSnapshot()) return;

    const ref = this._dialog.open<
      ConfirmDialogComponent,
      ConfirmDialogData,
      boolean
    >(ConfirmDialogComponent, {
      data: {
        title: 'accountSnapshot.confirmDeleteTitle',
        message: 'accountSnapshot.confirmDeleteMessage',
        confirmLabel: 'accountSnapshot.confirmDeleteAccept',
        cancelLabel: 'accountSnapshot.confirmDeleteCancel',
        danger: true,
      },
    });

    const confirmed = await ref.afterClosed().toPromise();
    if (!confirmed) return;

    await this._transactionService.deleteSnapshot(this.accountId());
    this.form.reset();
    this.hasSnapshot.set(false);
  }
}
