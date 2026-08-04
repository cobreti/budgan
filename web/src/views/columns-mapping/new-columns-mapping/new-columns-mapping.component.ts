import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatSelect } from '@angular/material/select';
import { MatOption } from '@angular/material/core';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { COLUMNS_MAPPING_SERVICE, ColumnsMappingService } from '@services/columns-mapping.service';
import {
  CSV_CONTENT_EXTRACTOR_SERVICE,
  CsvContentExtractorService,
} from '@services/csv-content-extractor.service';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { PageComponent } from '@components/page/page.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';

@Component({
  selector: 'app-new-columns-mapping',
  templateUrl: './new-columns-mapping.component.html',
  styleUrl: './new-columns-mapping.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    PageComponent,
    ReactiveFormsModule,
    MatFormField,
    MatLabel,
    MatError,
    MatInput,
    MatSelect,
    MatOption,
    MatButton,
    TranslatePipe,
    PageMenuComponent,
    PageBodyComponent,
    PageMenuButtonComponent,
  ],
})
export class NewColumnsMappingComponent {
  private readonly _columnsMappingService = inject<ColumnsMappingService>(COLUMNS_MAPPING_SERVICE);
  private readonly _csvExtractor = inject<CsvContentExtractorService>(
    CSV_CONTENT_EXTRACTOR_SERVICE,
  );
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);

  readonly csvHeaders = signal<string[]>([]);
  readonly selectedFileName = signal<string>('');
  readonly csvParseError = signal<string>('');

  readonly form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    cardNumberColumnIndex: new FormControl<number | null>(null, [Validators.required]),
    dateInscriptionColumnIndex: new FormControl<number | null>(null, [Validators.required]),
    amountColumnIndex: new FormControl<number | null>(null, [Validators.required]),
    descriptionColumnIndex: new FormControl<number | null>(null, [Validators.required]),
  });

  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const text = await file.text();
    const result = this._csvExtractor.extract(text);

    if (!result.success) {
      this.csvParseError.set(result.error);
      this.csvHeaders.set([]);
      this.selectedFileName.set('');
      return;
    }

    this.csvHeaders.set(result.value.header);
    this.selectedFileName.set(file.name);
    this.csvParseError.set('');
    this.form.controls.cardNumberColumnIndex.reset();
    this.form.controls.dateInscriptionColumnIndex.reset();
    this.form.controls.amountColumnIndex.reset();
    this.form.controls.descriptionColumnIndex.reset();
  }

  async onCreate(): Promise<void> {
    if (this.form.invalid) return;
    const {
      name,
      cardNumberColumnIndex,
      dateInscriptionColumnIndex,
      amountColumnIndex,
      descriptionColumnIndex,
    } = this.form.getRawValue();
    const headers = this.csvHeaders();
    const result = await this._columnsMappingService.save({
      name,
      cardNumberColumnIndex: cardNumberColumnIndex!,
      cardNumberColumnText: headers[cardNumberColumnIndex!],
      dateInscriptionColumnIndex: dateInscriptionColumnIndex!,
      dateInscriptionColumnText: headers[dateInscriptionColumnIndex!],
      amountColumnIndex: amountColumnIndex!,
      amountColumnText: headers[amountColumnIndex!],
      descriptionColumnIndex: descriptionColumnIndex!,
      descriptionColumnText: headers[descriptionColumnIndex!],
    });
    if (!result.success) {
      this.form.controls.name.setErrors({ nameExists: true });
      return;
    }
    await this._router.navigate([this._locale.currentLocale(), 'columns-mapping', result.value.id]);
  }

  onCancel(): void {
    this._router.navigate([this._locale.currentLocale()]);
  }
}
