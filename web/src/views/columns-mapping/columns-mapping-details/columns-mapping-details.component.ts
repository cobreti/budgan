import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ColumnsMapping } from '@models/columnsMappingModel';
import {
  COLUMNS_MAPPING_SERVICE,
  ColumnsMappingService,
} from '@services/columns-mapping.service';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';
import { PageComponent } from '@components/page/page.component';

@Component({
  selector: 'app-columns-mapping-details',
  templateUrl: './columns-mapping-details.component.html',
  styleUrl: './columns-mapping-details.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TranslatePipe, PageComponent, PageBodyComponent, PageMenuComponent, PageMenuButtonComponent],
})
export class ColumnsMappingDetailsComponent implements OnInit {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _service = inject<ColumnsMappingService>(COLUMNS_MAPPING_SERVICE);

  readonly mapping = signal<ColumnsMapping | undefined>(undefined);

  onBack(): void {
    this._router.navigate([this._locale.currentLocale()]);
  }

  async ngOnInit(): Promise<void> {
    const id = this._route.snapshot.params['columnsMappingId'];
    const entry = await this._service.getById(id);
    this.mapping.set(entry);
  }

  async onDelete(): Promise<void> {
    const m = this.mapping();
    if (!m) throw new Error('Mapping is undefined');
    await this._service.delete(m.id!);
    await this._router.navigate([this._locale.currentLocale()]);
  }
}
