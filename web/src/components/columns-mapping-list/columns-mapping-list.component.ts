import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import {
  COLUMNS_MAPPING_SERVICE,
  ColumnsMappingService,
} from '@services/columns-mapping.service';
import { ColumnsMapping } from '@models/columnsMappingModel';

@Component({
  selector: 'app-columns-mapping-list',
  templateUrl: './columns-mapping-list.component.html',
  styleUrl: './columns-mapping-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TranslatePipe],
})
export class ColumnsMappingListComponent {
  private readonly _service = inject<ColumnsMappingService>(COLUMNS_MAPPING_SERVICE);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);

  readonly mappings = signal<ColumnsMapping[]>([]);

  constructor() {
    this.refresh();
  }

  async refresh(): Promise<void> {
    this.mappings.set(await this._service.getList());
  }

  open(id: string): void {
    this._router.navigate([this._locale.currentLocale(), 'columns-mapping', id]);
  }
}
