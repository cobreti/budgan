import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatOption } from '@angular/material/core';
import { MatSelect } from '@angular/material/select';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { ACCOUNT_SERVICE, AccountService } from '@services/account.service';
import { COLUMNS_MAPPING_SERVICE, ColumnsMappingService } from '@services/columns-mapping.service';
import { ColumnsMapping } from '@models/columnsMappingModel';
import { AccountType } from '@models/accountModel';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { PageComponent } from '@components/page/page.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';

@Component({
  selector: 'app-new-account',
  templateUrl: './new-account.component.html',
  styleUrl: './new-account.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [PageComponent, ReactiveFormsModule, MatFormField, MatLabel, MatError, MatInput, MatSelect, MatOption, MatButton, TranslatePipe, PageBodyComponent, PageMenuComponent, PageMenuButtonComponent],
})
export class NewAccountComponent {
  private readonly _accountService = inject<AccountService>(ACCOUNT_SERVICE);
  private readonly _columnsMappingService = inject<ColumnsMappingService>(COLUMNS_MAPPING_SERVICE);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);

  readonly columnsMappings = signal<ColumnsMapping[]>([]);

  protected readonly accountTypes: AccountType[] = ['debit', 'credit'];

  readonly form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    accountType: new FormControl<AccountType>('debit', { nonNullable: true, validators: [Validators.required] }),
    columnsMappingId: new FormControl<string | null>(null, [Validators.required]),
  });

  constructor() {
    this._loadMappings();
  }

  private async _loadMappings(): Promise<void> {
    this.columnsMappings.set(await this._columnsMappingService.getList());
  }

  accountTypeLabelKey(type: AccountType): string {
    return type === 'credit' ? 'newAccount.accountTypeCredit' : 'newAccount.accountTypeDebit';
  }

  async onCreate(): Promise<void> {
    if (this.form.invalid) return;
    const { name, columnsMappingId, accountType } = this.form.getRawValue();
    const result = await this._accountService.create(name, columnsMappingId!, accountType);
    if (!result.success) {
      this.form.controls.name.setErrors({ nameExists: true });
      return;
    }
    await this._router.navigate([this._locale.currentLocale()]);
  }

  onCancel(): void {
    this._router.navigate([this._locale.currentLocale()]);
  }
}
