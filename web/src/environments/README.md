# Environment configuration

This directory holds the environment files used to configure the Angular build for each **build type** (`pwa` vs `server`) and **mode** (`dev` vs `prod`). `src/environments/environment.ts` is the file imported throughout the app (e.g. `@/utils/build-type.ts`); at build time Angular CLI swaps it out for the right variant via `fileReplacements` in [angular.json](../../angular.json).

| File | Used by build configuration | `fileReplacements` target |
|---|---|---|
| `environment.ts` | default (dev-time editor/type-checking baseline) | — |
| `environment.pwa.prod.ts` | `production`, `pwa` | replaces `environment.ts` |
| `environment.pwa.dev.ts` | `pwa-dev` | replaces `environment.ts` |
| `environment.server.prod.ts` | `server` | replaces `environment.ts` |
| `environment.server.dev.ts` | `server-dev` | replaces `environment.ts` |

Each file exports a single `environment` object typed against the shape declared in `environment.ts`.

## Fields

- **`production`** (`boolean`) — standard Angular flag; `true` disables dev-mode checks (`isDevMode()`), which also gates registration of the service worker (see `web/CLAUDE.md`).
- **`buildType`** (`BuildType`, `'pwa' | 'server'`) — read via `getCurrentBuildType()` / `isServerBuild()` / `isPWABuild()` in `src/utils/build-type.ts`. Selects which service implementations get used for tokens that have both a PWA and server `Impl` (e.g. `FILE_SERVICE`).
- **`useServiceWorker`** (`boolean`) — enables the Angular service worker (`ngsw-worker.js`). Only ever `true` for `environment.pwa.prod.ts` — the PWA build is the only one that ships offline/installable behavior; server builds serve through a backend and don't register a service worker.
- **`apiBaseUrl`** (`string | null`) — base URL the app calls for backend API requests (`BudganSvr`).
  - `null` in the `pwa.*` files: the PWA build talks to IndexedDB directly (see `IndexdbService`) and has no backend API to call.
  - `'http://localhost:3000/api'` in `environment.server.dev.ts`: local dev, pointed at wherever `BudganSvr` is running in Development (see the `watch:server` launch profile).
  - `'/api'` in `environment.server.prod.ts`: relative path, since in production the Angular app and `BudganSvr` are served from the same origin.
- **`auth`** (`AuthConfig | null`, see `src/types/auth-config.ts`) — MSAL configuration for signing in against the API's Azure AD app registration. `null` in the `pwa.*` files (no backend calls, so no token needed). Populated only in the `server.*` files:
  - `clientId` — the SPA's app registration client ID.
  - `authority` — MSAL authority URL.
  - `apiScope` — the scope requested when acquiring a token to call `BudganSvr` (`api://<client-id>/access_as_user`).

## Azure AD auth config — regular Entra ID tenant, matches the server

```ts
auth: {
  clientId: '<client-id>',
  authority: 'https://login.microsoftonline.com/<tenant-id>/',
  apiScope: 'api://<client-id>/access_as_user',
}
```

This `auth` config targets a regular workforce/organizational **Microsoft Entra ID tenant** — the app registrations behind `clientId` / `apiScope` live in that tenant, so sign-in targets the organization's own accounts — see [Server/BudganSvr/README.md](../../../Server/BudganSvr/README.md#azuread-configuration) for the full explanation.

### `authority` format

```
https://login.microsoftonline.com/<tenant-id>/
```

- `https://login.microsoftonline.com/` — the standard Entra ID issuer host used by regular work/school tenants.
- `<tenant-id>` — the tenant's GUID, appended as a path segment (mirrors `AzureAd:TenantId` on the server). Use `common`/`organizations` instead of a specific GUID only if the app registration is meant to accept accounts from any organization.

Because both sides authenticate against the same tenant:

- `authority`'s `<tenant-id>` must match `AzureAd:Instance` / `AzureAd:TenantId` in `Server/BudganSvr/appsettings*.json`.
- `clientId` here is the **SPA's** app registration, which is distinct from the **API's** `AzureAd:ClientId` on the server — `apiScope` (`api://<api-client-id>/access_as_user`) is what ties the two together: it must reference the API app registration's client ID and its exposed `access_as_user` scope.
- All values are placeholders in source control (`<client-id>`, `<tenant-id>`). Real values come from the App Registrations in the Azure Portal and should not be committed.
