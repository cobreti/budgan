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

Configuration section passed to `AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))` in `Program.cs`, used to validate bearer tokens on incoming API requests. See the dedicated section below — this configuration is tailored for an **External Tenant**, not a regular Azure AD tenant.

## AzureAd configuration — External Tenant (not a regular tenant)

```json
"AzureAd": {
  "Instance": "https://<external tenant domain>.ciamlogin.com/",
  "TenantId": "<tenant-id>",
  "ClientId": "<client-id>"
}
```

This app registration lives in a **Microsoft Entra External ID tenant** (formerly "Azure AD CIAM" — a customer-facing "external tenant"), not a regular workforce/organizational Entra ID tenant. This shows up in a couple of ways that are easy to "fix" incorrectly if you're used to standard Entra ID setups:

- **`Instance` uses the CIAM domain format**: `https://<external tenant domain>.ciamlogin.com/`, instead of the standard `https://login.microsoftonline.com/`. This is the issuer endpoint for External ID tenants specifically — it is a different product from a normal work/school tenant, even though both are "Azure AD"/"Entra ID".
- **No `common`/`organizations`/`consumers` alias**: because this tenant is meant for external/customer identities (e.g. email or social sign-up) rather than an organization's own employees, there is no multi-tenant `TenantId: "common"` fallback to accept accounts from arbitrary orgs. `Instance` + `TenantId` must point at this specific External tenant.
- **Client and server must agree on the same tenant**: the Angular frontend (`web/src/environments/environment.server.dev.ts` / `environment.server.prod.ts`) configures MSAL with a matching `authority: https://<external tenant domain>.ciamlogin.com/<tenant-id>`. Both sides need the same External Tenant domain and `TenantId` for issued tokens to validate against this API.
- **Placeholders**: `TenantId` and `ClientId` are committed as placeholders (`<tenant-id>`, `<client-id>`). Real values come from the App Registration created in the External tenant via the Azure Portal, and should be supplied through a local `appsettings.Development.json` override or environment variables — never committed with real values.
