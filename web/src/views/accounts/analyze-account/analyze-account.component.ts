import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { MatButton } from '@angular/material/button';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatIcon } from '@angular/material/icon';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { LOCALE_SERVICE, LocaleService } from '@services/locale.service';
import { ACCOUNT_ANALYSIS_SERVICE, AccountAnalysisService } from '@services/account-analysis.service';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';
import { PageMenuComponent } from '@components/page-menu/page-menu.component';
import { PageMenuButtonComponent } from '@components/page-menu/page-menu-button/page-menu-button.component';
import { PageComponent } from '@components/page/page.component';
import { PageBodyComponent } from '@components/page-body/page-body.component';

@Component({
  selector: 'app-analyze-account',
  templateUrl: './analyze-account.component.html',
  styleUrl: './analyze-account.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    TranslatePipe,
    DecimalPipe,
    PageComponent,
    PageBodyComponent,
    PageMenuComponent,
    PageMenuButtonComponent,
    MatButton,
    MatCheckbox,
    MatIcon,
    MatProgressSpinner,
    MatTableModule,
  ],
})
export class AnalyzeAccountComponent {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _locale = inject<LocaleService>(LOCALE_SERVICE);
  private readonly _analysisService = inject<AccountAnalysisService>(ACCOUNT_ANALYSIS_SERVICE);

  readonly accountId = this._route.snapshot.params['accountId'] as string;

  readonly isAnalyzing = signal(false);
  readonly isApplying = signal(false);
  readonly errorKey = signal<string | null>(null);
  readonly recurringTransactions = signal<AccountRecurringTransactionModel[]>([]);
  readonly selectedIds = signal<Set<string>>(new Set());
  readonly persistedIds = signal<Set<string>>(new Set());

  readonly allSelected = computed(() => {
    const rows = this.recurringTransactions();
    return rows.length > 0 && rows.every((t) => this.selectedIds().has(t.id));
  });

  readonly someSelected = computed(
    () => this.recurringTransactions().some((t) => this.selectedIds().has(t.id)) && !this.allSelected(),
  );

  readonly hasChanges = computed(() => {
    const persisted = this.persistedIds();
    const selected = this.selectedIds();
    if (persisted.size !== selected.size) return true;
    for (const id of selected) {
      if (!persisted.has(id)) return true;
    }
    return false;
  });

  readonly columns = [
    'select',
    'description',
    'firstOccurrenceDate',
    'lastOccurrenceDate',
    'transactionCount',
    'averageAmount',
    'periodInDays',
  ];

  constructor() {
    this._loadExisting();
  }

  private async _loadExisting(): Promise<void> {
    const results = await this._analysisService.getRecurringTransactions(this.accountId);
    this.recurringTransactions.set(results);
    this.selectedIds.set(new Set(results.map(t => t.id)));
    this.persistedIds.set(new Set(results.map(t => t.id)));
  }

  async onAnalyze(): Promise<void> {
    this.isAnalyzing.set(true);
    this.errorKey.set(null);
    const previousIds = new Set(this.recurringTransactions().map(t => t.id));
    const result = await this._analysisService.analyzeAccount(this.accountId);
    if (result.success) {
      this.recurringTransactions.set(result.value);
      this.selectedIds.set(new Set(result.value.filter(t => previousIds.has(t.id)).map(t => t.id)));
    } else {
      this.errorKey.set('analyzeAccount.error');
    }
    this.isAnalyzing.set(false);
  }

  async onApply(): Promise<void> {
    this.isApplying.set(true);
    this.errorKey.set(null);
    const selected = this.selectedIds();
    const toPersist = this.recurringTransactions().filter(t => selected.has(t.id));
    const result = await this._analysisService.applyAnalysis(this.accountId, toPersist);
    this.isApplying.set(false);
    if (result.success) {
      this.persistedIds.set(new Set(toPersist.map(t => t.id)));
      this._router.navigate([this._locale.currentLocale(), 'account', this.accountId]);
    } else {
      this.errorKey.set('analyzeAccount.error');
    }
  }

  isSelected(id: string): boolean {
    return this.selectedIds().has(id);
  }

  toggleOne(id: string): void {
    const next = new Set(this.selectedIds());
    if (next.has(id)) {
      next.delete(id);
    } else {
      next.add(id);
    }
    this.selectedIds.set(next);
  }

  toggleAll(): void {
    if (this.allSelected()) {
      this.selectedIds.set(new Set());
    } else {
      this.selectedIds.set(new Set(this.recurringTransactions().map((t) => t.id)));
    }
  }

  onBack(): void {
    this._router.navigate([this._locale.currentLocale(), 'account', this.accountId]);
  }
}
