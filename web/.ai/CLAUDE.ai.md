# Budgan — Claude Working Context

Angular 21 + TypeScript SPA. CSV bank-statement import, journals, accounts, transactions with running balances. All data in-browser (IndexedDB via Dexie). PWA. **Active app:** `App/Budgan/`. Run all commands from there.

## Commands

| Task | Command |
|---|---|
| Dev server | `ng serve` (port 4200) |
| Build | `ng build` |
| Unit tests | `ng test` (Vitest + jsdom) |
| Format | `npx prettier --write .` |

## Layout (src/)

```
components/  app, header (+header-page-title), main, main-menu (+main-menu-button),
             page, page-body, page-menu (+page-menu-button),
             journal-list, account-list, columns-mapping-list,
             account-snapshot, account-transactions-table, confirm-dialog
views/       Home, journals (list, new, details), columns-mapping (new, details),
             accounts (account-home + tabs: details/transactions/graphs;
                       new, import-file, save-account),
             save, load, samples
services/    interface + InjectionToken + Impl pattern (see table)
Models/      plain interfaces only
types/       result.ts → Result<T>
utils/       date.ts (ISO parse/format, previousIsoDay)
```

**Path aliases:** `@components/*`, `@models/*`, `@services/*`, `@views/*`, `@app-types/*`, `@/*` → `src/*`.

## Service tokens (registered in `src/components/app/app.config.ts`)

| Token | Impl |
|---|---|
| `ID_GENERATOR_SERVICE` | `IdGeneratorServiceImpl` |
| `LOCALE_SERVICE` | `LocaleServiceImpl` |
| `THEME_SERVICE` | `ThemeServiceImpl` |
| `JOURNAL_SERVICE` | `JournalServiceImpl` |
| `COLUMNS_MAPPING_SERVICE` | `ColumnsMappingServicePwaImpl` |
| `ACCOUNT_SERVICE` | `AccountServicePwaImpl` |
| `FILE_SERVICE` | `FileServiceImpl` |
| `ACCOUNT_TRANSACTION_SERVICE` | `AccountTransactionServiceImpl` |
| `CSV_CONTENT_EXTRACTOR_SERVICE` | `CsvContentExtractorServiceImpl` |
| `BUDGAN_EXPORT_SERVICE` | `BudganExportServiceImpl` |

**Non-tokenized (inject by class — do not add more like this):** `IndexdbService` (extends Dexie), `MainMenuService` (`isOpen`, `toggleMenu`, `close`), `PageService` (`title`, `setTitle`).

## Service pattern (mandatory for new services)

```typescript
export interface MyService { doThing(): void; }
export const MY_SERVICE = new InjectionToken<MyService>('MyService');

@Injectable({ providedIn: 'root' })
export class MyServiceImpl implements MyService {
  private readonly _dep = inject<OtherService>(OTHER_SERVICE);  // inject() in body, never ctor
  doThing(): void { /* ... */ }
}
```
Register: `{ provide: MY_SERVICE, useClass: MyServiceImpl }` in `app.config.ts`.

## Result pattern (no throws for expected errors)

```typescript
Result<T> = { success: true; value: T } | { success: false; error: string };
```
- Mutations return `Promise<Result<T>>`.
- Read helpers (`getList`, `getListByAccount`) return `Promise<T[]>` directly.
- `getById(id)` may throw — callers treat result as guaranteed.

## Component rules

- Standalone (do NOT set `standalone: true` — default in v20+).
- `changeDetection: ChangeDetectionStrategy.OnPush` on every `@Component`.
- State: `signal()` / `computed()`. No `@Input`/`@Output` — use `input()` / `output()`.
- Control flow: `@if`, `@for`, `@switch`. Never `*ngIf`/`*ngFor`/`*ngSwitch`.
- Bindings: `class.x` / `style.x`. Never `ngClass` / `ngStyle`.
- Host: `host` object on `@Component`. Never `@HostBinding` / `@HostListener`.
- DI: `inject()` in body. Never constructor parameters.
- `TranslatePipe` in `imports` for any template using `| translate`.
- Static images: `<img ngSrc="/path" width="N" height="N" alt="…">` via `NgOptimizedImage`. Never plain `<img src>`. (Base64 exempt — unsupported.)
- Interactive elements: `data-testid="kebab-case"`.

## Theming

Both light and dark MUST work. Use `--mat-sys-*` tokens. Fallbacks must be `inherit`, never literal RGB:
```scss
color: var(--mat-sys-on-surface-variant, inherit);  // ✓
color: var(--mat-sys-on-surface-variant, rgba(0,0,0,0.6));  // ✗ breaks dark
```

## IndexedDB schema — v8 (`IndexdbService`, db `'budgan'`)

| Table | Indexes | Entity |
|---|---|---|
| `workspaces` | `&id, &name` | `JournalModel` |
| `columnMappings` | `&id, &name` | `ColumnsMapping` |
| `accounts` | `&id, &name` | `AccountModel` |
| `files` | `&id, filename, accountId` | `fileModel` |
| `accountTransactions` | `&id, accountId, fileId` | `AccountTransactionModel` |

- Schema changes: add a new `version(N+1).stores({...})`. **Never edit a published version.**
- Helpers: `clearAll()`, `replaceAll(AllDataExportPayload)`.

## Domain types

```typescript
JournalModel    { id; name }
AccountModel    { id; name; columnsMappingId; referenceBalance?: AccountReferenceBalance }
AccountReferenceBalance { date: string; balance: number }
ColumnsMapping  { id?; name;
                  cardNumberColumnIndex; cardNumberColumnText?;
                  dateInscriptionColumnIndex; dateInscriptionColumnText?;
                  amountColumnIndex; amountColumnText?;
                  descriptionColumnIndex; descriptionColumnText? }
fileModel       { id; accountId; filename; content; insertionDate: Date }
AccountTransactionModel {
  id;            // normal: `${cardNumber}|${date}|${amount}|${description}`
                 // snapshot: `snapshot|${accountId}`  (one per account)
  fileId; accountId; cardNumber; dateInscriptionAsString; amount;
  balance?; balanceDateOffset?;     // filled by recalculateBalances
  description;
  recordType: 'normal' | 'snapshot';
}

CsvContentExtractionResult { delimiter; headerRowIndex; header: string[]; rows: CsvJsonRecord[] }
CsvJsonRecord = Record<string, string>

AccountExportPayload  { version; account; columnsMapping; files; transactions }
AllDataExportPayload  { version; columnsMappings; accounts; files; transactions }
```

## Balance algorithm — `AccountTransactionService.recalculateBalances(accountId)`

Call after **every** transaction mutation (`create`, `setSnapshot`, `deleteSnapshot`).
- **With snapshot:** anchor at snapshot row. Forward pass on rows `>= snapshotDate` adds `amount`, offset `+1, +2…`. Backward pass on rows `< snapshotDate` stores balance BEFORE subtracting `amount`, offset `-1, -2…`.
- **No snapshot:** opening balance = 0; forward pass adds `amount`, offset `+1, +2…`.
- Side effects: `bulkPut` all rows, `AccountService.setReferenceBalance(accountId, { date: previousIsoDay(earliest.date), balance: earliest.balance - earliest.amount })`, then bumps `transactionsVersion` signal so dependent views re-fetch.

## CSV import pipeline (`ImportFileComponent` → `/{locale}/account/:accountId/import-file`)

1. `file.text()`
2. `CsvContentExtractorService.extract(text)` — detects delimiter (`, ; \t |`), scores header row, normalizes headers, returns `Result<…>`.
3. Resolve account + its `ColumnsMapping`.
4. `FileService.create()` → store raw CSV. Loop rows → `AccountTransactionService.create()`. Composite id; duplicates rejected by `&id` unique index.
5. `AccountTransactionService.recalculateBalances(accountId)`.
6. Navigate back to `/{locale}/account/:accountId`.

## Routes (locale-prefixed; `src/components/app/app.routes.ts`)

```
/                                      defaultLocaleGuard → /<browser-locale>
/:locale                               localeGuard (en|fr)
  /                                    HomeComponent
  /journals                            JournalsComponent
  /journal/new                         NewJournalComponent
  /journal/:journalId                  JournalDetailsComponent
  /columns-mapping/new                 NewColumnsMappingComponent
  /columns-mapping/:columnsMappingId   ColumnsMappingDetailsComponent
  /account/new                         NewAccountComponent
  /account/:accountId                  AccountHomeComponent (tabs: details/transactions/graphs)
  /account/:accountId/import-file      ImportFileComponent
  /account/:accountId/save             SaveAccountComponent
  /save                                SaveComponent
  /load                                LoadComponent
  /samples                             SamplesComponent
/**                                    → /en
```
- `localeGuard`: validates locale ∈ `LocaleService.supportedLocales`, then calls `LocaleService.setLocale()` + `TranslateService.use()`.
- Navigate via `this._router.navigate([this._locale.currentLocale(), …])`.

## i18n

- Files: `public/assets/i18n/{en,fr}.json`. **Never `src/assets/`** — CLI only serves `public/`.
- Templates: `{{ 'menu.newJournal' | translate }}`. Dot-namespaced keys.
- **Every key MUST be in both `en.json` and `fr.json`.**

## Save / load (`.bdg` files via File System Access API)

- `BudganExportService.pickSaveFile`/`pickLoadFile` → `FileSystemFileHandle | null`.
- `buildAccountPayload(accountId)` / `buildAllDataPayload()`.
- `parseAllDataPayload(text)` returns `Result<…>` — `parse-error` | `invalid-format`.
- Load flow: confirm dialog → `IndexdbService.replaceAll(payload)`.
- Bumping `.bdg` schema: increment `version`, extend the validator, keep older `version` readable.

## PWA & charts

- Service worker registered only in prod: `provideServiceWorker('ngsw-worker.js', { enabled: !isDevMode() })`.
- Manifest: `public/manifest.webmanifest`. Icons: `public/icons/Budgan{48,180,256,512}.png`.
- Graphs: `ng2-charts`, registered globally via `provideCharts(withDefaultRegisterables())`.
- Bundled demo data: `/samples` → fetch `/assets/samples/sample.json` → `parseAllDataPayload` → confirm → `replaceAll`.

## Naming

| | |
|---|---|
| Files | `kebab-case` (`journal.service.ts`) |
| Interface | PascalCase (`JournalService`) |
| Token | SCREAMING_SNAKE_CASE (`JOURNAL_SERVICE`) |
| Impl | PascalCase + `Impl` (`JournalServiceImpl`) |
| Component class | PascalCase + `Component` |
| i18n key | dot-namespaced (`menu.newJournal`) |
| Test selector | `data-testid="kebab-case"` |

## Checklists

**Add a service:** interface + `InjectionToken` + `Impl` → register in `app.config.ts` → `inject()` deps in body.
**Add a persistent table/index:** new `version(N+1).stores({...})` entry; declare the `Table`/`EntityTable` field; assign in constructor.
**Add a component:** standalone (no flag), `OnPush`, signals, `data-testid` on interactive elements, `TranslatePipe` in `imports` if needed.
**Add a route:** under `/:locale` in `app.routes.ts`; navigate with `[locale, …]`.
**Add i18n:** key in both `en.json` and `fr.json`.
**Add a static image:** under `public/`; reference via `NgOptimizedImage` with explicit `width`/`height`.
**Set header title in a view:** inject `PageService`, call `setTitle('i18n.key')`.
**Mutate transactions:** call `AccountTransactionService.recalculateBalances(accountId)` after the write.
**Bump `.bdg`:** version field + validator update + back-compat read path.

## Tools

`Tools/StatementGenerator/` — Node 20+ utility that generates mock CSV statements. Outside the Angular bundle.
