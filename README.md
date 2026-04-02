# FBZ Encyclopedia
**Fantasy B'zaar Encyclopedia System**

A full-stack ASP.NET Core MVC web application for exploring the British Library Comics Unmasked dataset — 70,000+ comic records with search, filtering, external API enrichment, and a CI/CD pipeline to AWS Elastic Beanstalk.

---

## ✨ Features

| Area | Highlights |
|------|-----------|
| Search | Title, author, genre, year range, language, edition, resource type, content type |
| External APIs | Google Books · Open Library · Comic Vine · Wikipedia · QR Code |
| Accounts | Register / login / logout via ASP.NET Core Identity |
| Saved Lists | Persist search results across sessions in SQLite |
| Staff Tools | Flag records, analytics dashboard, dataset updates |
| Kiosk Mode | Touch-friendly UI for public terminals |
| CI/CD | GitHub Actions → AWS Elastic Beanstalk (auto-deploy on push to `main`) |

---

## 🌐 External API Integrations

Each comic detail page is enriched with live data from the following APIs:

**Google Books** — matches by ISBN or title, returns publisher, page count, description, and preview links.

**Open Library** — fetches cover art, first publish year, subjects, and related ISBNs.

**Comic Vine** — looks up comic-specific metadata including publisher, deck summary, and series information.

**Wikipedia** — looks up the first author via `/api/rest_v1/page/summary/{author}`, returns a biography, thumbnail, and link to the full article. No API key required.

**QR Code (api.qrserver.com)** — generates a scannable QR code on the detail page linking to the Wikipedia or Google Books page. No API key required.

---

## 🔒 Security

- All string search inputs capped at 200 characters; `PageSize` clamped to 100
- External API calls skipped if title or author name exceeds 300 characters
- Fixed-window rate limiting on `/Dataset` — 30 requests per minute per IP, returns 429 when exceeded
- API keys loaded from environment variables / config, not hardcoded
- `ILogger` on all API services — failures logged as warnings, never crash the app
- API results cached in memory for 1 hour per comic to reduce external calls

---

## Search & Display

- Loads `names.csv` (~70K rows) into memory for fast in-process filtering
- Combines duplicate rows into single records with merged values (years, ISBNs, authors)
- Sorting: A–Z / Z–A via pluggable strategy classes
- Grouping: None / Author / Year
- Paging with configurable page size (max 100)

---

## Accounts & Persistence

- ASP.NET Core Identity — register, login, logout with hashed passwords
- Saved Lists — save search results to SQLite, accessible across devices and sessions
- Session List — temporary add / remove / clear / export to CSV within a session
- Kiosk mode — `?kiosk=1` activates a touch-optimised layout; Finish/Exit clears the session

---

## Staff Features

All staff routes require the `Staff` role.

| Route | Purpose |
|-------|---------|
| `/staff/flags` | Flag records for review with a reason; unflag when resolved |
| `/staff/analytics` | Top 10 queries, top 10 results, comics in 100+ searches |
| `/staff/dataset` | Trigger a live dataset update from the British Library source |

---

## 🚀 CI/CD Pipeline

Every push to `main` triggers:

```
GitHub Actions: Restore → Build → Test
→ dotnet publish
→ Copy names.csv + Procfile into artefact
→ Verify artefact (fails fast if files missing)
→ Zip → Upload to S3
→ Create Elastic Beanstalk application version
→ Deploy to EB environment
```

Infrastructure is defined in `infra/terraform/` (AWS, us-east-1, t3.micro, .NET 9 on Amazon Linux 2023).

---

## ▶️ Run Locally

**Prerequisites:** .NET 9 SDK

```bash
cd FBZSystemMvc
dotnet restore
dotnet run
```

Open `http://localhost:5236` in your browser.

| URL | Description |
|-----|-------------|
| `/` | Home |
| `/Dataset` | Search |
| `/Dataset?kiosk=1` | Kiosk mode |
| `/Privacy` | Privacy policy |
| `/staff/flags` | Staff — flagged records |
| `/staff/analytics` | Staff — analytics dashboard |
| `/staff/dataset` | Staff — dataset update |

---

## 🧪 Automated Tests

```bash
dotnet test FBZSystemMvc.Tests/FBZSystemMvc.Tests.csproj
```

19 tests across three suites:

- **API service tests** — GoogleBooksService, OpenLibraryService, WikipediaService tested with a fake HTTP handler (no real network calls). Covers valid responses, empty responses, API down, edge cases.
- **Analytics tests** — AnalyticsService logic against an in-memory SQLite database
- **Smoke tests** — key routes return 200 (Home, Dataset, Kiosk)

---

## Project Structure

```
FBZSystemMvc/
├── Controllers/          MVC controllers (staff routes under Controllers/Staff/)
├── Data/                 names.csv dataset
├── Domain/               core domain types (Comic, SearchQuery)
├── Migrations/           EF Core migrations
├── Models/               view models
├── Persistence/          EF Core DbContext and entities
├── Repositories/         CSV data access with hot-reload support
├── Services/
│   ├── ExternalApis/     Google Books, Open Library, Comic Vine, Wikipedia
│   ├── Persistence/      analytics and saved comics services
│   └── DatasetUpdates/   background dataset update service
├── Strategies/           sort and group strategies
├── Views/                Razor views
└── wwwroot/              static assets
infra/terraform/          AWS infrastructure definitions
.github/workflows/        CI/CD pipeline
```

---

## SOLID Principles

| Principle | Example |
|-----------|---------|
| Single Responsibility | `ComicRepositoryCsv` loads CSV · `SearchService` handles filtering · `AnalyticsService` handles analytics |
| Open/Closed | New sort or group strategies can be added without changing `SearchService` |
| Liskov Substitution | Any `ISortStrategy` or `IGroupingStrategy` can be swapped in transparently |
| Interface Segregation | Small focused interfaces — `IComicRepository`, `ISearchService`, `IAnalyticsService` |
| Dependency Inversion | Controllers and services depend on abstractions; all wiring in `Program.cs` |

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | C# / .NET 9 — ASP.NET Core MVC |
| Database | EF Core + SQLite |
| Auth | ASP.NET Core Identity |
| Dataset | Deedle (CSV parsing) |
| Testing | xUnit |
| CI/CD | GitHub Actions |
| Hosting | AWS Elastic Beanstalk + S3 |
| Infrastructure | Terraform |

---

## Dataset

**British Library — Comics Unmasked** (Researcher Format)
License: CC0 1.0 Universal (Public Domain)
Source: [bl.uk/collection-metadata/downloads](https://www.bl.uk/collection-metadata/downloads)

The British Library does not endorse this application.

---

## Author

**Aman** — HND Cloud & AI Computing
