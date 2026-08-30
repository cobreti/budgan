import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatSelect } from '@angular/material/select';
import { MatOption } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { AUTH_SERVICE } from '@services/auth.service';
import { isServerBuild } from '@/utils/build-type';
import { Role } from '@/types/role';
import { COLUMNS_MAPPING_SERVICE, ColumnsMappingService } from '@services/columns-mapping.service';
import {
  CSV_CONTENT_EXTRACTOR_SERVICE,
  CsvContentExtractorService,
  CsvJsonRecord,
} from '@services/csv-content-extractor.service';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { PageComponent } from '@components/page/page.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';
import {
  DemoStatementPickerComponent,
  DemoStatementPickerData,
} from '@components/demo-statement-picker/demo-statement-picker.component';
import {
  DATE_FORMAT_PRESETS,
  DateFormatParseResult,
  formatIsoDate,
  parseDateWithFormatDetailed,
} from '@/utils/date';

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
  private readonly _dialog = inject(MatDialog);
  private readonly _authService = inject(AUTH_SERVICE);

  readonly canChooseCsvFile = computed(
    () => !isServerBuild() || this._authService.roles().includes(Role.PersonalDataAllowed),
  );

  readonly csvHeaders = signal<string[]>([]);
  readonly csvRows = signal<CsvJsonRecord[]>([]);
  readonly selectedFileName = signal<string>('');
  readonly selectedFileSource = signal<'native' | 'demo' | null>(null);
  readonly csvParseError = signal<string>('');
  readonly dateFormatPreview = signal<DateFormatParseResult | null>(null);
  readonly cardNumberSample = signal<string>('');
  readonly dateInscriptionSample = signal<string>('');
  readonly amountSample = signal<string>('');
  readonly descriptionSample = signal<string>('');
  readonly dateFormatPresets = DATE_FORMAT_PRESETS;

  readonly form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    cardNumberColumnIndex: new FormControl<number | null>(null, [Validators.required]),
    dateInscriptionColumnIndex: new FormControl<number | null>(null, [Validators.required]),
    amountColumnIndex: new FormControl<number | null>(null, [
      Validators.required,
      this.amountSampleIsNumericValidator(),
    ]),
    descriptionColumnIndex: new FormControl<number | null>(null, [Validators.required]),
    dateFormat: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, this.dateFormatMatchesSampleValidator()],
    }),
  });

  constructor() {
    this.form.controls.dateFormat.valueChanges.subscribe(() => this.updateDateFormatPreview());
    this.form.controls.dateInscriptionColumnIndex.valueChanges.subscribe(() => {
      this.updateDateInscriptionSample();
      this.autoDetectDateFormat();
      this.form.controls.dateFormat.updateValueAndValidity();
      this.updateDateFormatPreview();
    });
    this.form.controls.cardNumberColumnIndex.valueChanges.subscribe(() => this.updateCardNumberSample());
    this.form.controls.amountColumnIndex.valueChanges.subscribe(() => this.updateAmountSample());
    this.form.controls.descriptionColumnIndex.valueChanges.subscribe(() => this.updateDescriptionSample());
  }

  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    await this.processFile(file, 'native');
  }

  async onLoadDemoStatement(): Promise<void> {
    const ref = this._dialog.open<DemoStatementPickerComponent, DemoStatementPickerData, File[]>(
      DemoStatementPickerComponent,
      { data: { multiple: false } },
    );
    const files = await ref.afterClosed().toPromise();
    if (!files || files.length === 0) return;
    await this.processFile(files[0], 'demo');
  }

  private async processFile(file: File, source: 'native' | 'demo'): Promise<void> {
    const text = await file.text();
    const result = this._csvExtractor.extract(text);

    if (!result.success) {
      this.csvParseError.set(result.error);
      this.csvHeaders.set([]);
      this.csvRows.set([]);
      this.selectedFileName.set('');
      this.selectedFileSource.set(null);
      return;
    }

    this.csvHeaders.set(result.value.header);
    this.csvRows.set(result.value.rows);
    this.selectedFileName.set(file.name);
    this.selectedFileSource.set(source);
    this.csvParseError.set('');
    this.form.controls.cardNumberColumnIndex.reset();
    this.form.controls.dateInscriptionColumnIndex.reset();
    this.form.controls.amountColumnIndex.reset();
    this.form.controls.descriptionColumnIndex.reset();
    this.form.controls.dateFormat.reset('');
    this.updateDateFormatPreview();
  }

  private updateDateFormatPreview(): void {
    const pattern = this.form.controls.dateFormat.value.trim();
    const colIndex = this.form.controls.dateInscriptionColumnIndex.value;
    const rows = this.csvRows();

    if (!pattern || colIndex === null || rows.length === 0) {
      this.dateFormatPreview.set(null);
      return;
    }

    const sampleValue = this.sampleValueForColumn(colIndex);
    this.dateFormatPreview.set(parseDateWithFormatDetailed(sampleValue, pattern));
  }

  private sampleValueForColumn(colIndex: number | null): string {
    if (colIndex === null) return '';
    const headers = this.csvHeaders();
    const rows = this.csvRows();
    if (rows.length === 0) return '';
    return rows[0][headers[colIndex]] ?? '';
  }

  private updateCardNumberSample(): void {
    this.cardNumberSample.set(this.sampleValueForColumn(this.form.controls.cardNumberColumnIndex.value));
  }

  private updateDateInscriptionSample(): void {
    this.dateInscriptionSample.set(
      this.sampleValueForColumn(this.form.controls.dateInscriptionColumnIndex.value),
    );
  }

  private updateAmountSample(): void {
    this.amountSample.set(this.sampleValueForColumn(this.form.controls.amountColumnIndex.value));
  }

  private updateDescriptionSample(): void {
    this.descriptionSample.set(this.sampleValueForColumn(this.form.controls.descriptionColumnIndex.value));
  }

  private autoDetectDateFormat(): void {
    const colIndex = this.form.controls.dateInscriptionColumnIndex.value;
    const sample = this.sampleValueForColumn(colIndex);
    if (!sample) return;

    const match = DATE_FORMAT_PRESETS.find(
      (preset) => parseDateWithFormatDetailed(sample, preset.pattern).success,
    );
    if (match) {
      this.form.controls.dateFormat.setValue(match.pattern);
    }
  }

  private dateFormatMatchesSampleValidator(): ValidatorFn {
    return (control: AbstractControl<string>): ValidationErrors | null => {
      const pattern = control.value?.trim();
      const sample = this.dateInscriptionSample();
      if (!pattern || !sample) return null;
      return parseDateWithFormatDetailed(sample, pattern).success
        ? null
        : { dateSampleMismatch: true };
    };
  }

  private amountSampleIsNumericValidator(): ValidatorFn {
    return (control: AbstractControl<number | null>): ValidationErrors | null => {
      const sample = this.sampleValueForColumn(control.value);
      if (!sample) return null;
      const numericValue = parseFloat(sample.replace(',', '.'));
      return Number.isNaN(numericValue) ? { amountSampleNotNumeric: true } : null;
    };
  }

  formatPreviewDate(date: Date): string {
    return formatIsoDate(date);
  }

  async onCreate(): Promise<void> {
    if (this.form.invalid) return;
    const {
      name,
      cardNumberColumnIndex,
      dateInscriptionColumnIndex,
      amountColumnIndex,
      descriptionColumnIndex,
      dateFormat,
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
      dateFormat,
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
