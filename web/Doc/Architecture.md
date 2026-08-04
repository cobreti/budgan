# Budgan Architecture

## 1. Overview

Budgan is a single-page Angular application for importing CSV bank statements and managing them as journals and accounts. All data lives in the browser (IndexedDB) — there is no server component. Users can export/import their entire dataset to a `.bdg` file using the File System Access API.

- **Framework**: Angular 21 (standalone components, signals, `OnPush` change detection)
- **UI**: Angular Material 21 + Material Icons + Roboto
- **Persistence**: IndexedDB via Dexie 4
- **i18n**: `@ngx-translate/core` with HTTP loader (`en`, `fr`)
- **Build/test**: Angular CLI (`@angular/build`) + Vitest
- **Source root**: `App/Budgan/`

## 2. Source layout

```
App/Budgan/
├── angular.json            # CLI config; assets served from public/
├── package.json
├── public/
│   ├── assets/i18n/        # en.json, fr.json (runtime translation files)
│   ├── assets/samples/     # sample.json (.bdg dataset bundled with the app)
│   ├── icons/              # Budgan{48,180,256,512}.png (PWA + header brand)
│   └── manifest.webmanifest
└── src/
    ├── main.ts             # bootstrapApplication(App, appConfig)
    ├── styles.scss
    ├── components/         # Reusable building blocks
    │   ├── app/            # Root component, routes, DI config, locale guard
    │   ├── header/         # Toolbar + brand button + page-title sub-component
    │   ├── main/           # Hosts the <router-outlet>
    │   ├── main-menu/
    │   ├── page/ + page-menu/ + page-body/
    │   ├── journal-list/
    │   ├── columns-mapping-list/
    │   ├── account-list/
    │   ├── account-snapshot/         # Balance-anchor form for an account
    │   ├── account-transactions-table/
    │   └── confirm-dialog/
    ├── views/              # Routed pages
    │   ├── Home/
    │   ├── journals/       (list, new, details)
    │   ├── columns-mapping/(new, details)
    │   ├── accounts/       (account-home + tabs: details, transactions, graphs;
    │   │                    plus new, import-file, save-account)
    │   ├── load/
    │   ├── save/
    │   └── samples/        # Loads bundled sample.json into IndexedDB
    ├── services/           # Interface + Impl pattern, registered in app.config.ts
    ├── Models/             # Plain TypeScript data shapes
    ├── types/              # Shared utility types (Result<T>)
    └── utils/              # Pure helpers (date.ts: ISO date parse/format)
```

TypeScript path aliases (`tsconfig.json`): `@components/*`, `@models/*`, `@services/*`, `@views/*`, `@app-types/*`.

## 3. Application bootstrap

`main.ts` → `bootstrapApplication(App, appConfig)`.

`App` (`src/components/app/app.ts`) is a thin shell that renders:
- `<app-header>` — toolbar with menu toggle, brand button (Budgan icon + title — uses `NgOptimizedImage` against `/icons/Budgan48.png`), page-title slot, locale switcher, theme toggle
- `<mat-sidenav-container>` — holds `<app-main-menu>` and `<main>`
- `<main>` — hosts the `<router-outlet>`

`appConfig` (`src/components/app/app.config.ts`) wires the global providers:
- `provideBrowserGlobalErrorListeners()`, `provideRouter(routes)`, `provideHttpClient()`, `provideAnimations()`
- `provideNativeDateAdapter()` — Angular Material datepicker support
- `provideTranslateService` + `provideTranslateHttpLoader` (prefix `/assets/i18n/`, suffix `.json`)
- `provideCharts(withDefaultRegisterables())` — `ng2-charts` registration used by the account graphs view
- `provideServiceWorker('ngsw-worker.js', { enabled: !isDevMode() })` — PWA support
- All service tokens → their `Impl` classes (see §6)

### Shell composition

```mermaid
flowchart TD
    Main["main.ts<br/>bootstrapApplication"] --> App["App (root)"]
    App --> Header["app-header<br/>(menu, locale, theme)"]
    App --> SideNavContainer["mat-sidenav-container"]
    SideNavContainer --> SideNav["mat-sidenav"]
    SideNav --> MainMenu["app-main-menu"]
    SideNavContainer --> MainEl["&lt;main&gt;"]
    MainEl --> Outlet["router-outlet"]
    Outlet -.->|renders| Views["Routed view component<br/>(see §4)"]
```

## 4. Routing & i18n guards

Routes are locale-prefixed and defined in `src/components/app/app.routes.ts`.

```
/                                       → defaultLocaleGuard → /<browser-locale> (renders JournalsComponent during redirect)
/:locale                                → localeGuard
  /                                     HomeComponent
  /journals                             JournalsComponent
  /journal/new                          NewJournalComponent
  /journal/:journalId                   JournalDetailsComponent
  /columns-mapping/new                  NewColumnsMappingComponent
  /columns-mapping/:columnsMappingId    ColumnsMappingDetailsComponent
  /account/new                          NewAccountComponent
  /account/:accountId                   AccountHomeComponent  (Material tab group:
                                          details | transactions | graphs)
  /account/:accountId/import-file       ImportFileComponent
  /account/:accountId/save              SaveAccountComponent
  /save                                 SaveComponent
  /load                                 LoadComponent
  /samples                              SamplesComponent
/**                                     → /en
```

`locale.guard.ts`:
- `defaultLocaleGuard` redirects `/` to `/<detected-locale>` via `LocaleService.detectBrowserLocale()`.
- `localeGuard` validates `:locale` is in `LocaleService.supportedLocales` (`en` | `fr`), then calls both `LocaleService.setLocale()` and `TranslateService.use()` so reactive UI and translation pipe stay in sync.

All component-driven navigation builds links via `this._router.navigate([locale, ...])` using the current locale signal.

### Route tree

```mermaid
flowchart TD
    Root["/"] -->|defaultLocaleGuard| Locale["/:locale<br/>(localeGuard)"]
    Wildcard["/**"] -->|redirect| EnRedirect["/en"]

    Locale --> Home["/<br/>HomeComponent"]
    Locale --> Journals["/journals<br/>JournalsComponent"]
    Locale --> JournalNew["/journal/new<br/>NewJournalComponent"]
    Locale --> JournalDetails["/journal/:journalId<br/>JournalDetailsComponent"]
    Locale --> MappingNew["/columns-mapping/new<br/>NewColumnsMappingComponent"]
    Locale --> MappingDetails["/columns-mapping/:id<br/>ColumnsMappingDetailsComponent"]
    Locale --> AccountNew["/account/new<br/>NewAccountComponent"]
    Locale --> AccountHome["/account/:accountId<br/>AccountHomeComponent"]
    AccountHome --> AccTabs["Tabs: details | transactions | graphs"]
    Locale --> AccountImport["/account/:accountId/import-file<br/>ImportFileComponent"]
    Locale --> AccountSave["/account/:accountId/save<br/>SaveAccountComponent"]
    Locale --> Save["/save<br/>SaveComponent"]
    Locale --> Load["/load<br/>LoadComponent"]
    Locale --> Samples["/samples<br/>SamplesComponent"]
```

### View → reusable component dependencies

```mermaid
flowchart LR
    subgraph Views
        Home[HomeComponent]
        Journals[JournalsComponent]
        JournalDetails[JournalDetailsComponent]
        AccountHome[AccountHomeComponent]
        AccDetails[AccountDetailsComponent]
        AccTx[AccountTransactionsComponent]
        AccGraphs[AccountGraphsComponent]
        Import[ImportFileComponent]
        Mapping[ColumnsMappingDetailsComponent]
        SaveV[SaveComponent / SaveAccountComponent]
        LoadV[LoadComponent]
        SamplesV[SamplesComponent]
    end

    subgraph Reusable["Reusable components"]
        Page[page]
        PageBody[page-body]
        PageMenu[page-menu]
        PageMenuBtn[page-menu-button]
        JournalList[journal-list]
        AccountList[account-list]
        MappingList[columns-mapping-list]
        TxTable[account-transactions-table]
        Snapshot[account-snapshot]
        Confirm[confirm-dialog]
    end

    Home --> Page
    Home --> JournalList
    Home --> AccountList
    Home --> MappingList
    Journals --> Page
    Journals --> PageMenu
    Journals --> PageMenuBtn
    Journals --> JournalList
    JournalDetails --> Page
    JournalDetails --> PageMenu
    JournalDetails --> PageMenuBtn
    AccountHome --> Page
    AccountHome --> PageBody
    AccountHome --> PageMenu
    AccountHome --> PageMenuBtn
    AccountHome --> AccDetails
    AccountHome --> AccTx
    AccountHome --> AccGraphs
    AccDetails --> Snapshot
    AccTx --> TxTable
    Import --> Page
    Import --> PageMenu
    Import --> PageMenuBtn
    Mapping --> Page
    Mapping --> PageMenu
    Mapping --> PageMenuBtn
    SaveV --> Page
    SaveV --> PageMenu
    LoadV --> Page
    SamplesV --> Page
    SamplesV --> PageBody
    SamplesV --> PageMenu
    SamplesV --> PageMenuBtn
    JournalList -. delete .-> Confirm
    AccountList -. delete .-> Confirm
    MappingList -. delete .-> Confirm
    Snapshot -. delete .-> Confirm
    LoadV -. confirm replace .-> Confirm
    SamplesV -. confirm replace .-> Confirm
```

## 5. Domain model

Plain interfaces in `src/Models/` (no class methods, no behavior):

| Model | Purpose | Key fields |
|---|---|---|
| `JournalModel` | A user-named workspace | `id`, `name` |
| `AccountModel` | A bank account belonging to a user | `id`, `name`, `columnsMappingId` |
| `ColumnsMapping` | Maps CSV columns → semantic transaction fields | `id?`, `name`, `cardNumberColumnIndex` + `Text?`, `dateInscriptionColumnIndex` + `Text?`, `amountColumnIndex` + `Text?`, `descriptionColumnIndex` + `Text?` |
| `fileModel` | An imported CSV file (raw content kept) | `id`, `accountId`, `filename`, `content`, `insertionDate` |
| `AccountTransactionModel` | A single parsed transaction (or the snapshot anchor row) | `id`, `fileId`, `accountId`, `cardNumber`, `dateInscriptionAsString`, `amount`, `balance?`, `balanceDateOffset?`, `description`, `recordType` (`normal` \| `snapshot`) |

Snapshot rows live in the same `accountTransactions` table as normal rows. Their id is the literal `snapshot|<accountId>` (one per account) and `recordType = snapshot`. Normal transaction ids are the composite `${cardNumber}|${date}|${amount}|${description}`.

### Result type

`Result<T> = { success: true; value: T } | { success: false; error: string }` (`src/types/result.ts`). All fallible service operations return this shape rather than throwing; only "should never happen" lookups (e.g. `getById` for a known id) throw.

### Entity relationships

```mermaid
erDiagram
    JournalModel {
        string id PK
        string name
    }
    AccountModel {
        string id PK
        string name
        string columnsMappingId FK
    }
    ColumnsMapping {
        string id PK
        string name
        number cardNumberColumnIndex
        number dateInscriptionColumnIndex
        number amountColumnIndex
        number descriptionColumnIndex
    }
    fileModel {
        string id PK
        string accountId FK
        string filename
        string content
        Date insertionDate
    }
    AccountTransactionModel {
        string id PK
        string fileId FK
        string accountId FK
        string cardNumber
        string dateInscriptionAsString
        number amount
        number balance "running balance after recalculate"
        number balanceDateOffset "stable tie-break for same-date rows"
        string description
        enum recordType "normal | snapshot"
    }

    AccountModel ||--o{ fileModel : "owns"
    AccountModel ||--o{ AccountTransactionModel : "owns"
    AccountModel }o--|| ColumnsMapping : "uses"
    fileModel ||--o{ AccountTransactionModel : "produced"
```

> `JournalModel` is the `workspaces` table; it exists as a workspace container but is not currently linked to accounts/files/transactions at the type level.

## 6. Service layer

Every service follows the **interface + `InjectionToken` + `Impl` class** pattern. Components and other services inject by token, never by concrete class. All tokens are registered in `app.config.ts`.

| Token | Implementation | Responsibility |
|---|---|---|
| `ID_GENERATOR_SERVICE` | `IdGeneratorServiceImpl` | `crypto.randomUUID()` wrapper for entity ids |
| `LOCALE_SERVICE` | `LocaleServiceImpl` | Holds `currentLocale` signal; detects browser locale; lists supported locales |
| `THEME_SERVICE` | `ThemeServiceImpl` | `isDark` signal; toggles `dark-theme` class on `<html>`; persists in `localStorage` |
| `JOURNAL_SERVICE` | `JournalServiceImpl` | CRUD on `workspaces` table |
| `ACCOUNT_SERVICE` | `AccountServicePwaImpl` | CRUD on `accounts` table |
| `COLUMNS_MAPPING_SERVICE` | `ColumnsMappingServicePwaImpl` | CRUD on `columnMappings` table (create or update via `save()`) |
| `FILE_SERVICE` | `FileServiceImpl` | CRUD on `files` table; query by `accountId` |
| `ACCOUNT_TRANSACTION_SERVICE` | `AccountTransactionServiceImpl` | CRUD + paginated/sorted read on `accountTransactions`; snapshot CRUD (`setSnapshot` / `getSnapshot` / `deleteSnapshot`); `recalculateBalances(accountId)` walks forward/backward from the snapshot to fill every row's `balance` + `balanceDateOffset`. Exposes a `transactionsVersion` signal so views re-fetch after writes. |
| `CSV_CONTENT_EXTRACTOR_SERVICE` | `CsvContentExtractorServiceImpl` | Pure CSV → JSON parsing (delimiter detection, header scoring, row extraction) |
| `BUDGAN_EXPORT_SERVICE` | `BudganExportServiceImpl` | File System Access API; builds & reads `.bdg` payloads |

Two UI-state holders are intentionally **not** tokenized — they are pure local-state services and are injected by class:

| Service | State | Used by |
|---|---|---|
| `MainMenuService` | `isOpen` signal; `toggleMenu()` / `close()` | `App` shell sidenav, `<app-header>` menu button |
| `PageService` | `title` signal; `setTitle(string)` | All page views set their title; `<app-header-page-title>` reads it |

`IndexdbService` is also injected by class directly (it extends `Dexie`).

### Conventions
- Use `inject()` inside the `Impl` body, never constructor parameters.
- Services that mutate persistent state return `Promise<Result<T>>`.
- Read-only helpers (`getList`, `getListByAccount`) return `Promise<T[]>` directly.
- `getById` throws if the id is unknown — callers treat the result as guaranteed.

### Service dependency graph

```mermaid
flowchart TD
    subgraph UIState["UI-state services (no token)"]
        LocaleSvc[LocaleService *]
        ThemeSvc[ThemeService *]
        MenuSvc[MainMenuService]
        PageSvc[PageService]
    end

    subgraph Domain["Domain services (tokenized)"]
        IdGen[IdGeneratorService]
        Journal[JournalService]
        Account[AccountService]
        Mapping[ColumnsMappingService]
        FileSvc[FileService]
        Tx[AccountTransactionService]
    end

    subgraph Pure["Pure / IO"]
        Csv[CsvContentExtractorService]
        Export[BudganExportService]
    end

    IndexDB[(IndexdbService<br/>Dexie)]

    Journal --> IdGen
    Account --> IdGen
    Mapping --> IdGen
    FileSvc --> IdGen

    Journal --> IndexDB
    Account --> IndexDB
    Mapping --> IndexDB
    FileSvc --> IndexDB
    Tx --> IndexDB
    Tx --> Account

    Export --> Account
    Export --> Mapping
    Export --> FileSvc
    Export --> Tx
```

`*` = tokenized (`LOCALE_SERVICE`, `THEME_SERVICE`); `MainMenuService` / `PageService` are injected by class.

## 7. Persistence (Dexie / IndexedDB)

`IndexdbService` extends `Dexie('budgan')` and owns the schema. It is `providedIn: 'root'` and exposed as a concrete class (no token) since there is one obvious implementation.

**Current schema (version 8):**

| Table | Indexes | Stored entity |
|---|---|---|
| `workspaces` | `&id, &name` | `JournalModel` |
| `columnMappings` | `&id, &name` | `ColumnsMapping` |
| `accounts` | `&id, &name` | `accountModel` |
| `files` | `&id, filename, accountId` | `fileModel` |
| `accountTransactions` | `&id, accountId, fileId` | `AccountTransactionModel` |

Versions 1–8 are kept in `indexdb.service.ts` so existing databases can migrate forward. **Adding a new table or index requires a new `version(N).stores({ ... })` entry** — never edit a published version.

### Transactional helpers
- `clearAll()` — wipes every table in a single `rw` transaction. Used by the "Clear all" UI action.
- `replaceAll(payload)` — clears every table and bulk-adds the contents of a loaded `.bdg` payload (workspaces are not in the payload yet).

### Tables and indexes

```mermaid
flowchart LR
    subgraph DB["IndexedDB 'budgan' (v8)"]
        WS["workspaces<br/>&id, &name"]
        CM["columnMappings<br/>&id, &name"]
        AC["accounts<br/>&id, &name"]
        FI["files<br/>&id, filename, accountId"]
        TX["accountTransactions<br/>&id, accountId, fileId"]
    end
    AC -->|columnsMappingId| CM
    FI -->|accountId| AC
    TX -->|accountId| AC
    TX -->|fileId| FI
```

## 8. CSV import pipeline

The CSV → transactions flow is split across three services and the `ImportFileComponent`. The flow for `/{locale}/account/:accountId/import-file`:

1. **File selection** — user picks a `.csv`; component reads `file.text()`.
2. **Extraction** — `CsvContentExtractorService.extract(text)`:
   - Splits on `\r?\n`, ignores blank lines.
   - Detects delimiter by scoring `,` `;` `\t` `|` against most-common column count.
   - Picks header row by scoring the first 10 candidates (alpha chars, non-numeric, uniqueness).
   - Normalizes headers (`lowercase`, non-alphanumerics → `_`, dedupes with `_2`, `_3`...).
   - Returns `Result<{ delimiter, headerRowIndex, header, rows }>`.
3. **Mapping resolution** — component loads the account and its `ColumnsMapping`.
4. **Persistence** — creates a `fileModel` row holding the raw CSV content, then iterates `rows` and writes one `AccountTransactionModel` per parseable row. The transaction `id` is the composite `${cardNumber}|${date}|${amount}|${description}` — duplicates fail the `&id` unique index (the `add` simply rejects, so re-importing the same statement is a no-op).
5. **Balance recompute** — after the rows are written, the component (or follow-on snapshot edit) calls `AccountTransactionService.recalculateBalances(accountId)`, which fills `balance` + `balanceDateOffset` on every row anchored on the snapshot (or 0 if there is none).
6. **Navigation** — redirects back to `/{locale}/account/:accountId`.

```mermaid
sequenceDiagram
    actor U as User
    participant V as ImportFileComponent
    participant Csv as CsvContentExtractorService
    participant A as AccountService
    participant M as ColumnsMappingService
    participant F as FileService
    participant T as AccountTransactionService
    participant DB as IndexdbService

    U->>V: pick CSV file
    V->>V: file.text()
    V->>Csv: extract(text)
    Csv-->>V: Result<{delimiter, header, rows}>
    V->>A: getById(accountId)
    A->>DB: accounts.get(id)
    V->>M: getById(account.columnsMappingId)
    M->>DB: columnMappings.get(id)
    V->>F: create(accountId, name, content, now)
    F->>DB: files.add()
    F-->>V: Result<fileId>
    loop for each row
        V->>T: create(fileId, accountId, card, date, amount, desc)
        T->>DB: accountTransactions.add()
    end
    V->>T: recalculateBalances(accountId)
    T->>DB: bulkPut(updated rows)
    V-->>U: navigate /{locale}/account/:id
```

## 9. Save / load (.bdg export)

`BudganExportService` wraps the File System Access API (`showSaveFilePicker`, `showOpenFilePicker`) and the payload builders.

### Account-scoped payload (`AccountExportPayload`, version 1)
```
{ version, account, columnsMapping, files[], transactions[] }
```
Used by `SaveAccountComponent` to export a single account's data.

### Full-dataset payload (`AllDataExportPayload`, version 1)
```
{ version, columnsMappings[], accounts[], files[], transactions[] }
```
Used by `SaveComponent` (export everything except journals) and `LoadComponent`, which calls `IndexdbService.replaceAll(payload)` after the picker returns a valid file.

`readAllDataPayload()` validates the parsed JSON with `isAllDataExportPayload()` and returns `Result<...>` — invalid files surface as `parse-error` or `invalid-format` rather than throwing.

### Save flow (all data)

```mermaid
sequenceDiagram
    actor U as User
    participant V as SaveComponent
    participant E as BudganExportService
    participant Svc as Account/Mapping/File/Tx services
    participant FS as File System Access API

    U->>V: click Save
    V->>E: pickSaveFile(suggestedName)
    E->>FS: showSaveFilePicker()
    FS-->>E: FileSystemFileHandle
    V->>E: buildAllDataPayload()
    E->>Svc: getList() x4 (parallel)
    Svc-->>E: arrays
    E-->>V: AllDataExportPayload
    V->>E: writeJsonToFile(handle, payload)
    E->>FS: writable.write(JSON.stringify(...))
```

### Load flow (all data)

```mermaid
sequenceDiagram
    actor U as User
    participant V as LoadComponent
    participant E as BudganExportService
    participant FS as File System Access API
    participant DB as IndexdbService

    U->>V: click Load
    V->>E: pickLoadFile()
    E->>FS: showOpenFilePicker()
    FS-->>E: FileSystemFileHandle
    V->>E: readAllDataPayload(handle)
    E->>FS: handle.getFile().text()
    E-->>V: Result<AllDataExportPayload>
    alt success
        V->>DB: replaceAll(payload)
        DB->>DB: rw txn: clear all + bulkAdd
    else parse-error / invalid-format
        V-->>U: show error
    end
```

## 10. Account snapshot & running balances

A "snapshot" is a single known-true balance the user enters for an account at a specific date. It is stored as one extra row in `accountTransactions` with `recordType = snapshot` and `id = snapshot|<accountId>` (so there is at most one per account).

`AccountTransactionService.recalculateBalances(accountId)` is the engine that turns transactions into running balances. It is invoked by `setSnapshot`, `deleteSnapshot`, and whenever transactions are mutated.

```mermaid
flowchart LR
    subgraph Inputs["Inputs (per accountId)"]
        Snap["snapshot row<br/>(may be absent)"]
        Normals["normal rows<br/>(sorted by date, id)"]
    end

    subgraph WithSnap["With snapshot"]
        SD["snapshotDate"]
        After["rows >= snapshotDate<br/>forward pass: +amount, offset +1, +2..."]
        Before["rows <  snapshotDate<br/>backward pass: balance BEFORE -amount,<br/>offset -1, -2..."]
    end

    subgraph WithoutSnap["No snapshot"]
        FromZero["forward pass from 0<br/>balance += amount"]
    end

    Inputs --> WithSnap
    Inputs --> WithoutSnap
    WithSnap --> Write["bulkPut updated rows"]
    WithoutSnap --> Write
    Write --> Bump["transactionsVersion.update(v => v+1)"]
```

Views that depend on transactions (`AccountTransactionsComponent`, `AccountGraphsComponent`) read `transactionsVersion` via an `effect()` and re-fetch when it bumps.

## 11. PWA & graphs

- **PWA**: `provideServiceWorker('ngsw-worker.js', { enabled: !isDevMode() })` registers the Angular service worker only in production builds. Manifest at `public/manifest.webmanifest`; PNG icons at `public/icons/Budgan{48,180,256,512}.png`.
- **Charts**: `AccountGraphsComponent` uses `ng2-charts` (registered globally via `provideCharts(withDefaultRegisterables())`) to plot balance history from the recalculated `balance` field.
- **Bundled sample data**: `SamplesComponent` (`/samples`) fetches `/assets/samples/sample.json`, validates it through `BudganExportService.parseAllDataPayload()`, and replaces every IndexedDB table via `IndexdbService.replaceAll()` after a confirm dialog. Useful for first-run demos.

## 12. State management

There is no central store. State lives in four places:

1. **IndexedDB** (via Dexie) — the source of truth for all persistent domain data.
2. **Service-held signals** — small pieces of cross-component UI state:
   - `LocaleService.currentLocale`
   - `ThemeService.isDark`
   - `MainMenuService.isOpen`
   - `PageService.title` — current page title shown in the header.
3. **Cache-invalidation signal**: `AccountTransactionService.transactionsVersion` is bumped after every mutation (`create`, `setSnapshot`, `deleteSnapshot`, `recalculateBalances`); transaction-dependent views re-fetch by reading it in an `effect()`.
4. **Component-local signals** — page-specific view state (`signal()`, `computed()`). Async fetches store results into component signals; no observables-as-state.

```mermaid
flowchart LR
    subgraph Persistent["Persistent state"]
        DB[(IndexedDB / Dexie)]
    end
    subgraph Global["Global UI signals"]
        L[LocaleService.currentLocale]
        T[ThemeService.isDark]
        M[MainMenuService.isOpen]
        P[PageService.title]
        V[AccountTransactionService.transactionsVersion]
    end
    subgraph Local["Per-component signals"]
        C1[journal/account/file<br/>view state]
        C2[form & error signals]
    end

    DB <-->|services| C1
    L --> Header
    L --> Views[All routed views]
    T --> Header
    T --> RootHtml["html.dark-theme"]
    M --> AppShell["App sidenav"]
    P --> Header
    V --> TxViews["AccountTransactions /<br/>AccountGraphs"]
```

## 13. Component conventions

Enforced across the codebase (see `App/Budgan/.claude/CLAUDE.md` for the full list):

- Standalone components (do **not** set `standalone: true` — it's the default in v20+).
- `changeDetection: ChangeDetectionStrategy.OnPush` on every `@Component`.
- Signals only — no `@Input`/`@Output` decorators; use `input()` / `output()`.
- Native control flow (`@if`, `@for`, `@switch`) — no `*ngIf`/`*ngFor`/`*ngSwitch`.
- No `ngClass`/`ngStyle` — use `class.x`/`style.x` bindings.
- No `@HostBinding`/`@HostListener` — use the `host` object on the decorator.
- `inject()` in the body, not constructor parameters.
- Components using `| translate` must add `TranslatePipe` to their `imports`.
- Static `<img>` must use `NgOptimizedImage` with explicit `width`/`height` (see the brand button in `<app-header>` for the pattern). Inline base64 images are out of scope for `NgOptimizedImage`.
- Interactive elements carry `data-testid="kebab-case"` for tests.

## 14. Theming

Light/dark theming uses Material 3 CSS system variables. `ThemeService` toggles a `dark-theme` class on `<html>` and persists the choice in `localStorage`.

**Rule**: any color in component styles must use `--mat-sys-*` variables. Fallbacks must be `inherit`, not literal RGB — a hard-coded fallback like `rgba(0,0,0,0.6)` silently breaks dark mode.

```scss
color: var(--mat-sys-on-surface-variant, inherit);  /* correct */
```

## 15. Internationalization

- Loader: `provideTranslateHttpLoader({ prefix: '/assets/i18n/', suffix: '.json' })`.
- Translation files live in `public/assets/i18n/` — Angular CLI only serves `public/`, so files under `src/assets/` are unreachable at runtime.
- Templates use `{{ 'menu.newJournal' | translate }}`. Keys are dot-namespaced and must be present in **both** `en.json` and `fr.json`.
- `localeGuard` calls `TranslateService.use(locale)` on every route activation, so the language follows the URL.

## 16. Testing & tooling

- **Unit tests**: Vitest with jsdom. Run via `ng test` (Angular CLI builder).
- **Formatting**: Prettier (`npx prettier --write .`).
- **TypeScript**: strict mode, plus `noImplicitOverride`, `noPropertyAccessFromIndexSignature`, `strictTemplates`.
- **Path aliases**: declared in `tsconfig.json` (`@components/*`, `@models/*`, `@services/*`, `@views/*`, `@app-types/*`, plus `@/*` → `src/*`).
- **PWA**: built via the Angular service worker. The service worker is **disabled in dev** (`isDevMode()` short-circuits registration); a production build (`ng build`) ships `ngsw-worker.js` and `manifest.webmanifest`.

## 17. Tools directory

`Tools/StatementGenerator/` is a standalone Node 20+ utility that generates mock CSV bank statements for testing the import pipeline. It is not part of the Angular bundle.

## 18. Extending the application

Checklist for the most common changes:

| Change | Where |
|---|---|
| New service | Interface + `InjectionToken` + `Impl` class in `src/services/`. Register provider in `app.config.ts`. |
| New entity needing an id | Inject `ID_GENERATOR_SERVICE` and call `generateId()`. |
| New persisted table or index | Add a new `version(N).stores({ ... })` entry to `IndexdbService`; keep prior versions intact. |
| Fallible operation | Return `Result<T>`, do not throw. |
| New component | Standalone (no flag), `OnPush`, signals for state, `data-testid` on interactive nodes. |
| New route | Add under `/:locale` in `app.routes.ts`; use the locale signal when building navigation arrays. |
| New i18n key | Add to both `public/assets/i18n/en.json` and `fr.json`. |
| New `.bdg` field | Bump `version` in the payload type, extend the `parseAllDataPayload` validator, and handle the older `version` on load. |
| New static image | Place under `public/`; reference with `NgOptimizedImage` (`<img ngSrc="/path" width="…" height="…">`). |
| New page that needs a header title | Inject `PageService` in the view and call `setTitle('i18n.key')` (translation happens at the consumer). |
| New account-transaction mutation | After writing, call `AccountTransactionService.recalculateBalances(accountId)` so running balances stay consistent. |