# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Restore & run locally (auto-applies migrations, seeds Staff role)
cd FBZSystemMvc && dotnet restore && dotnet run
# App available at http://localhost:5236

# Build release
dotnet build FBZSystemMvc.csproj -c Release

# Run all tests
dotnet test

# Run unit tests only
dotnet test FBZSystemMvc.Tests/FBZSystemMvc.Tests.csproj -c Release

# Run UI tests only (Selenium — requires app to be running)
dotnet test FBZSystemMvc.UITests/FBZSystemMvc.UITests.csproj

# Publish for deployment
dotnet publish FBZSystemMvc/FBZSystemMvc.csproj -c Release -o publish
```

## Architecture

This is an ASP.NET Core MVC (.NET 9) web application backed by SQLite + EF Core. It serves a search interface over the British Library Comics Unmasked CSV dataset (~70K records).

**Layers:**

- `Controllers/` — MVC controllers. Staff routes under `Controllers/Staff/` require the "Staff" role.
- `Services/` — Business logic. `SearchService` is the core. External API enrichment services (`GoogleBooksService`, `OpenLibraryService`, `ComicVineService`, `EmailService`) are called from `DatasetController` for the detail view.
- `Repositories/` — `ComicRepositoryCsv` loads `Data/names.csv` into memory via the Deedle library. `ReloadableComicRepository` wraps it to support hot-reload via `DatasetUpdateHostedService`.
- `Strategies/` — Pluggable `ISortStrategy` and `IGroupingStrategy` implementations. Registered as singletons and injected into `SearchService` as `IEnumerable<T>`.
- `Persistence/` — EF Core `ApplicationDbContext` with all SQLite-backed entities (analytics, saved lists, flags, identity).
- `Domain/` — `Comic` record type and `Search` model used throughout.

**Lifetimes matter:**

- `IComicRepository` → **Singleton** (CSV loaded once; hot-reload swaps the underlying data atomically)
- `ISortStrategy` / `IGroupingStrategy` implementations → **Singleton** (stateless)
- `ApplicationDbContext` → **Scoped** (one per request)
- `HttpClient`-based API services → registered via `IHttpClientFactory`

**Data flow:**
CSV (`Data/names.csv`) → `ComicRepositoryCsv` → `SearchService` (filter/sort/group) → `DatasetController` → Razor views

**Session vs persistence:**
- Session list (add/remove/export) — stored in `ISession`, cleared on logout
- Saved search lists — stored in SQLite `SavedSearchList`/`SavedSearchListItem`, require login
- Analytics (`SearchAnalyticsEvent`, `QueryStat`, `ComicResultStat`) — auto-tracked on every search

**CSV parsing note:**
Fields in `names.csv` use semicolons to separate multi-values within a cell. Commas within values are preserved. `ComicRepositoryCsv` handles this parsing.

**Database migrations** are auto-applied at startup in `Program.cs`. The "Staff" role is seeded automatically; to promote a user to Staff, assign the role via the database or a staff management endpoint.

## Deployment

CI/CD is GitHub Actions (`.github/workflows/deploy-eb.yml`) deploying to AWS Elastic Beanstalk on every push to `main`. The pipeline: build → test → publish → zip → upload to S3 → create EB version → swap environment.

Infrastructure is defined in `infra/terraform/` (AWS provider, us-east-1, t3.micro, .NET 9 on Amazon Linux 2023). Required GitHub Secrets: `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_ACCOUNT_ID`.

The `Procfile` (`web: dotnet FBZSystemMvc.dll`) is used by Elastic Beanstalk. The `PORT` environment variable is respected at startup.

## Key configuration

- `appsettings.json` — DB connection string (`fbz.db` SQLite), dataset update settings, Comic Vine API key, SendGrid API key placeholder
- `appsettings.Development.json` — Google Books API key, dev-mode overrides
- Dataset update is driven by `DatasetUpdate:ZipUrl` in config; `DatasetUpdateHostedService` polls every 24 hours by default

## API keys — required environment variables

| Key | Where to set | Notes |
|-----|--------------|-------|
| `SendGrid__ApiKey` | Environment variable or User Secrets | Free tier: 100 emails/day. Must also verify a sender domain/address in the SendGrid dashboard before any mail is delivered. Set `from` address in `EmailService.cs` (`FromEmail` constant) to match your verified sender. |
| `ComicVine__ApiKey` | Environment variable or User Secrets | Register at comicvine.gamespot.com |
| `GoogleBooks__ApiKey` | `appsettings.Development.json` or env var | Optional — unauthenticated requests work but are rate-limited |

**Never commit real API keys to source control.** Use `dotnet user-secrets` locally:
```bash
cd FBZSystemMvc
dotnet user-secrets set "SendGrid:ApiKey" "SG.your-key-here"
```
On AWS Elastic Beanstalk, set keys as environment properties in the EB console (they map to `SendGrid__ApiKey` with double-underscore notation).
