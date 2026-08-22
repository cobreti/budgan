# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

**Budgan** is an Angular + TypeScript financial application for CSV bank statement import and journal management. The active app lives in `App/Budgan/`.

All commands are run from the `App/Budgan/` directory.

## Commands

```bash
# Development
ng serve                # Start dev server (default port 4200)

# Build
ng build               # Angular CLI build

# Unit tests
ng test                # Vitest unit tests

# Code quality
npx prettier --write . # Format with Prettier
```

## Architecture

```
App/Budgan/src/
├── components/         # Reusable Angular components
│   ├── app/            # Root component, routing config, app config, locale guards
│   ├── account-list/
│   ├── account-snapshot/
│   ├── account-transactions-table/
│   ├── columns-mapping-list/
│   ├── confirm-dialog/
│   ├── header/                     # + header-page-title sub-component
│   ├── journal-list/
│   ├── main-menu/
│   │   └── main-menu-button/
│   ├── main/
│   ├── page/
│   ├── page-body/
│   └── page-menu/
│       └── page-menu-button/
├── Models/             # Plain TypeScript interfaces (data shapes)
├── services/           # Injectable services (interface + Impl pattern)
├── types/              # Shared utility types (Result<T>)
├── utils/              # Pure helpers (date.ts)
└── views/              # Routed page components
    ├── Home/
    ├── accounts/       # account-home + tabs (details, transactions, graphs);
    │                   # new-account, import-file, save-account
    ├── columns-mapping/
    ├── journals/
    ├── load/
    ├── samples/
    └── save/
```

**Data flow for CSV import:**
1. User loads a file → component calls `CsvContentExtractorService`
2. `CsvContentExtractorServiceImpl` detects delimiter, finds header row, parses to typed rows
3. Column mapping UI lets user assign CSV columns to semantic fields
4. `ColumnsMappingService` persists the mapping to IndexedDB via `IndexdbService` (Dexie)

## Mandatory patterns

### Services (Angular DI)

Always define a TypeScript interface for the contract, an `InjectionToken`, and an `Impl` class:

```typescript
// my-feature.service.ts
export interface MyFeatureService {
  doThing(): void;
}

export const MY_FEATURE_SERVICE = new InjectionToken<MyFeatureService>('MyFeatureService');

@Injectable({ providedIn: 'root' })
export class MyFeatureServiceImpl implements MyFeatureService {
  doThing(): void { /* ... */ }
}
```

- Use `inject()` inside the `Impl` class — never constructor injection.
- Register every new token in `src/components/app/app.config.ts`:

```typescript
{ provide: MY_FEATURE_SERVICE, useClass: MyFeatureServiceImpl }
```

**Current service tokens registered in `app.config.ts`:**

| Token | Impl |
|---|---|
| `ID_GENERATOR_SERVICE` | `IdGeneratorServiceImpl` |
| `LOCALE_SERVICE` | `LocaleServiceImpl` |
| `THEME_SERVICE` | `ThemeServiceImpl` |
| `JOURNAL_SERVICE` | `JournalServiceImpl` |
| `COLUMNS_MAPPING_SERVICE` | `ColumnsMappingServicePwaImpl` |
| `ACCOUNT_SERVICE` | `AccountServicePwaImpl` |
| `FILE_SERVICE` | `FileServicePwaImpl` / `FileServiceServerImpl` |
| `ACCOUNT_TRANSACTION_SERVICE` | `AccountTransactionServiceImpl` |
| `CSV_CONTENT_EXTRACTOR_SERVICE` | `CsvContentExtractorServiceImpl` |
| `BUDGAN_EXPORT_SERVICE` | `BudganExportServiceImpl` |

**Non-tokenized singletons** (injected by class, intentionally — they are pure UI-state holders):

| Class | Purpose |
|---|---|
| `MainMenuService` | `isOpen` signal for the sidenav; `toggleMenu()` / `close()` |
| `PageService` | `title` signal used by `<app-header-page-title>`; `setTitle(string)` |
| `IndexdbService` | Extends Dexie — owns IndexedDB schema and access for all domain services |

Do not add new non-tokenized services. New domain services must follow the interface + `InjectionToken` + `Impl` pattern.

### Result pattern (no throwing for expected errors)

```typescript
import { Result } from '../types/result';

save(item: MyModel): Promise<Result<MyModel>> {
  // ...
  return { success: true, value: item };
  return { success: false, error: 'name-exists' };
}

// Caller
const result = await service.save(item);
if (result.success) { /* result.value is typed */ }
```

`Result<T>` is defined as `{ success: true; value: T } | { success: false; error: string }`.

### Theming — light and dark mode

All UI changes must work correctly in both light and dark mode.

- Use Angular Material CSS variables (`--mat-sys-*`) for all colors — never hardcode color values.
- When a fallback is needed, use `inherit` rather than a static color (e.g. `rgba(0,0,0,0.6)` breaks in dark mode).
- Correct: `color: var(--mat-sys-on-surface-variant, inherit)`
- Wrong: `color: var(--mat-sys-on-surface-variant, rgba(0, 0, 0, 0.6))`

### Angular component rules

- **Standalone components only** — do NOT set `standalone: true` (it is the default in Angular v20+).
- Set `changeDetection: ChangeDetectionStrategy.OnPush` in every `@Component` decorator.
- Use signals (`signal()`, `computed()`) for all local state.
- Use `input()` and `output()` functions instead of `@Input`/`@Output` decorators.
- Use native control flow (`@if`, `@for`, `@switch`) — never `*ngIf`, `*ngFor`, `*ngSwitch`.
- Do NOT use `ngClass` or `ngStyle` — use `class` and `style` bindings instead.
- Do NOT use `@HostBinding` / `@HostListener` — put host bindings in the `host` object of `@Component`.
- Use `inject()` in the component body, not constructor parameters.
- Add `TranslatePipe` to the `imports` array of any component that uses `| translate`.
- Use `NgOptimizedImage` (`<img ngSrc="…" width="…" height="…" alt="…">`) for static images under `public/`. Do not use a plain `<img src>` for shipped assets. Inline base64 images are exempt (`NgOptimizedImage` does not support them).

### Persistence — IndexedDB via Dexie

`IndexdbService` (extends `Dexie('budgan')`) owns all database access. Current schema is **version 8**:

| Table | Schema | Entity type |
|---|---|---|
| `workspaces` | `&id, &name` | `JournalModel` |
| `columnMappings` | `&id, &name` | `ColumnsMapping` |
| `accounts` | `&id, &name` | `AccountModel` |
| `files` | `&id, filename, accountId` | `fileModel` |
| `accountTransactions` | `&id, accountId, fileId` | `AccountTransactionModel` |

Services inject `IndexdbService` directly to read/write their respective tables.

**Schema upgrades**: never edit a published `version(N).stores({...})` entry. Add a new `version(N+1).stores({...})` entry to migrate forward.

**Transactional helpers** on `IndexdbService`:
- `clearAll()` — wipes every table in a single `rw` transaction.
- `replaceAll(payload)` — clears every table and bulk-adds the contents of an `AllDataExportPayload` (used by `/load` and `/samples`).

### Naming conventions

| Entity | Convention |
|---|---|
| Files | `kebab-case` (e.g. `journal.service.ts`, `journal-list.component.ts`) |
| Service interface | `PascalCase` (e.g. `JournalService`) |
| Service token | `SCREAMING_SNAKE_CASE` (e.g. `JOURNAL_SERVICE`) |
| Service implementation | `PascalCase` + `Impl` suffix (e.g. `JournalServiceImpl`) |
| Components | `PascalCase` + `Component` suffix |
| i18n keys | Dot-namespaced (`menu.newJournal`) |
| Test selectors | `data-testid="kebab-case"` on all interactive/testable elements |

### Routing

Routes are locale-prefixed. Current route tree:

```
/                                      → defaultLocaleGuard (redirects to /<browser-locale>)
/:locale                               → localeGuard
  /                                    HomeComponent
  /journals                            JournalsComponent
  /journal/new                         NewJournalComponent
  /journal/:journalId                  JournalDetailsComponent
  /columns-mapping/new                 NewColumnsMappingComponent
  /columns-mapping/:columnsMappingId   ColumnsMappingDetailsComponent
  /account/new                         NewAccountComponent
  /account/:accountId                  AccountHomeComponent (tabs: details, transactions, graphs)
  /account/:accountId/import-file      ImportFileComponent
  /account/:accountId/save             SaveAccountComponent
  /save                                SaveComponent
  /load                                LoadComponent
  /samples                             SamplesComponent
/**                                    → redirects to /en
```

`localeGuard` validates the locale param (must be `en` or `fr`), calls `LocaleService.setLocale()`, and calls `TranslateService.use()`. `defaultLocaleGuard` redirects `/` to the browser's detected locale.

Construct links using the Angular Router:

```typescript
this.router.navigate([locale, 'journals']);
```

### i18n

Localization uses `@ngx-translate/core` with an HTTP loader.

- Translation files: `public/assets/i18n/en.json` and `public/assets/i18n/fr.json`  
  **Do NOT place them under `src/assets/`** — Angular CLI only serves from `public/`.
- All user-visible strings: `{{ 'some.key' | translate }}` in templates.
- Nested keys: `{ "menu": { "newJournal": "New Journal" } }` → `'menu.newJournal' | translate`.
- Every key must be present in **both** `en.json` and `fr.json`.

## Domain key types

```typescript
// Models
JournalModel    { id: string; name: string }
AccountModel    { id: string; name: string; columnsMappingId: string;
                  referenceBalance?: AccountReferenceBalance }
AccountReferenceBalance { date: string; balance: number }
ColumnsMapping  { id?: string; name: string;
                  cardNumberColumnIndex: number;        cardNumberColumnText?: string;
                  dateInscriptionColumnIndex: number;   dateInscriptionColumnText?: string;
                  amountColumnIndex: number;            amountColumnText?: string;
                  descriptionColumnIndex: number;       descriptionColumnText?: string }
fileModel       { id: string; accountId: string; filename: string;
                  content: string; insertionDate: Date }
AccountTransactionModel {
  id: string;                // normal: `${cardNumber}|${date}|${amount}|${description}`
                             // snapshot: `snapshot|${accountId}`
  fileId: string; accountId: string; cardNumber: string;
  dateInscriptionAsString: string; amount: number;
  balance?: number;           // populated by recalculateBalances
  balanceDateOffset?: number; // tiebreaker for same-date rows
  description: string;
  recordType: 'normal' | 'snapshot'
}

// CSV extraction
CsvContentExtractionResult { delimiter: string; headerRowIndex: number;
                             header: string[]; rows: CsvJsonRecord[] }
CsvJsonRecord              Record<string, string>

// Export payloads (.bdg files)
AccountExportPayload  { version, account, columnsMapping, files, transactions }
AllDataExportPayload  { version, columnsMappings, accounts, files, transactions }

// Result
Result<T>  { success: true; value: T } | { success: false; error: string }
```

After mutating transactions, call `AccountTransactionService.recalculateBalances(accountId)` so running `balance` and the account's `referenceBalance` stay consistent. The snapshot row is the anchor used by the algorithm.

## Checklist for new features

1. **New service** → interface + `InjectionToken` + `Impl` class; register token in `app.config.ts`
2. **New entity needing an ID** → inject `ID_GENERATOR_SERVICE` and call `generateId()`
3. **Persistence** → add a new `version(N).stores({ ... })` entry to `IndexdbService`; never edit a published version
4. **Fallible operation** → return `Result<T>`, not `throw`
5. **New component** → standalone, `OnPush`, signals for state, `data-testid` on all interactive elements
6. **New route** → add to `app.routes.ts` under `/:locale`
7. **New i18n text** → add key to both `en.json` and `fr.json`
8. **New static image** → place under `public/`; reference with `NgOptimizedImage` and explicit `width`/`height`
9. **New page that wants a header title** → inject `PageService` and call `setTitle('i18n.key')` from the view
10. **New `.bdg` field** → bump `version` in the payload type, extend `parseAllDataPayload`, and keep older `version` readable
11. **Mutating account transactions** → call `AccountTransactionService.recalculateBalances(accountId)` after the write so `balance` + `referenceBalance` stay in sync

## PWA & charts

- The Angular service worker is registered only in production builds (`provideServiceWorker('ngsw-worker.js', { enabled: !isDevMode() })`). PWA assets live at `public/manifest.webmanifest` and `public/icons/Budgan{48,180,256,512}.png`.
- The graphs tab uses `ng2-charts`; charts are registered globally via `provideCharts(withDefaultRegisterables())`.
