# BudganSvr configuration

This file documents the configuration keys used in `appsettings.json` and `appsettings.Development.json`.

## Configuration sections

### `Logging`

Standard ASP.NET Core log level configuration. `appsettings.Development.json` additionally enables `Debug` level for `Microsoft.AspNetCore.Authentication` and `Microsoft.IdentityModel`, which is useful when troubleshooting Azure AD token validation locally.

### `AllowedHosts`

Standard ASP.NET Core host filtering middleware setting. `*` means no restriction.

### `ConnectionStrings:DefaultConnection`

SQL Server connection string consumed by `AddInfrastructure` (`BudganInfra`) to configure `DataContext`. In `appsettings.Development.json` this points at the local SQL Server container started via `Database/Start-MssqlContainer.ps1` (`localhost,1433`, database `Budgan`, `sa`/`P@ssword`).

### `AllowedOrigins`

Array of origins used to build the `BudganCorsPolicy` CORS policy in `Program.cs`. The base `appsettings.json` leaves this empty (fail-closed). `appsettings.Development.json` allows `http://localhost:4200` and `http://localhost:4250` for the local Angular dev server.

### `AzureAd`

Configuration section passed to `AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))` in `Program.cs`, used to validate bearer tokens on incoming API requests. See the dedicated section below.

## AzureAd configuration

```json
"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "TenantId": "<tenant-id>",
  "ClientId": "<client-id>"
}
```

This app registration lives in a regular workforce/organizational **Microsoft Entra ID tenant**.

- **`Instance`**: `https://login.microsoftonline.com/`, the standard Entra ID issuer host.
- **`TenantId`**: the tenant's GUID. Use `common`/`organizations` instead only if the app registration should accept accounts from any organization.
- **Client and server must agree on the same tenant**: the Angular frontend (`web/src/environments/environment.server.dev.ts` / `environment.server.prod.ts`) configures MSAL with a matching `authority: https://login.microsoftonline.com/<tenant-id>/`. Both sides need the same `TenantId` for issued tokens to validate against this API.
- **Placeholders**: `TenantId` and `ClientId` are committed as placeholders (`<tenant-id>`, `<client-id>`). Real values come from the App Registration in the Azure Portal, and should be supplied through a local `appsettings.Development.json` override or environment variables — never committed with real values.
