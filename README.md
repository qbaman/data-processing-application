# FBZ Encyclopedia

A full-stack ASP.NET Core MVC web application for exploring the British Library Comics Unmasked dataset — 70,000+ comic records with search, filtering, external API enrichment, and a CI/CD pipeline to AWS Elastic Beanstalk.

---

## ✨ Features

| Area | Highlights |
|------|-----------|
| Search | Title, author, genre, year range, language, edition, resource type, content type |
| Comic Detail | Enhanced layout with cover image, description, AI summary, structured metadata, external links |
| Related Comics | Auto-suggests up to 6 similar comics by author, genre/topic, and year proximity |
| Related Videos | YouTube search for comic reviews and explainers, shown as thumbnail cards |
| AI Summary | On-demand OpenAI-generated catalogue description with 24-hour caching |
| Recently Viewed | Session-tracked strip of the last 5 comics visited |
| External APIs | Google Books · Open Library · Comic Vine · Wikipedia · YouTube · QR Code |
| Newsletter | Footer subscription prototype (UI demo) |
| Accounts | Register / login / logout via ASP.NET Core Identity |
| Saved Lists | Persist search results across sessions in SQLite |
| Staff Tools | Flag records, analytics dashboard, dataset updates |
| Kiosk Mode | Touch-friendly UI for public terminals |
| CI/CD | GitHub Actions → AWS Elastic Beanstalk (auto-deploy on push to `main`) |

---

## 🌐 External API Integrations

Each comic detail page is enriched with live data from the following APIs:

**OpenAI (gpt-4o-mini)** — on-demand AI-written catalogue summary. Click "Generate Summary" on any detail page. Results cached for 24 hours. Requires `OpenAI:ApiKey`.

**YouTube Data API v3** — searches for comic reviews, explainers, and reading guides by title. Displays up to 5 video thumbnails linking to YouTube. Costs 100 quota units per search (10,000 free/day). Requires `YouTube:ApiKey`.

**Google Books** — matches by ISBN or title, returns publisher, page count, description, and preview links.

**Open Library** — fetches cover art, first publish year, subjects, and related ISBNs.

**Comic Vine** — looks up comic-specific metadata including publisher, deck summary, and series information. Requires `ComicVine:ApiKey`.

**Wikipedia** — looks up the first author via `/api/rest_v1/page/summary/{author}`, returns a biography, thumbnail, and link to the full article. No API key required.

**QR Code (api.qrserver.com)** — generates a scannable QR code on the detail page linking to the Wikipedia or Google Books page. No API key required.

---

## 🔒 Security

**HTTP Security Headers** — applied to every response via middleware:

| Header | Value |
|--------|-------|
| `X-Content-Type-Options` | `nosniff` — prevents MIME-type sniffing |
| `X-Frame-Options` | `SAMEORIGIN` — blocks clickjacking |
| `Referrer-Policy` | `strict-origin-when-cross-origin` — limits referrer leakage |
| `Permissions-Policy` | camera, microphone, and geolocation disabled |
| `Content-Security-Policy` | Restricts resource origins to `'self'` and known CDN/image hosts |

**Application-level controls:**

- All string search inputs capped at 200 characters; `PageSize` clamped to 100
- External API calls skipped if title or author name exceeds 300 characters
- Fixed-window rate limiting on `/Dataset` — 30 requests per minute per IP, returns 429 when exceeded
- API keys loaded from environment variables / user secrets, never hardcoded
- `ILogger` on all API services — failures logged as warnings, never crash the app
- API results cached in memory (1 hour for most APIs, 24 hours for AI summaries)
- Anti-forgery tokens on all state-changing forms (ASP.NET Core default)
- HSTS enabled in production

---

## Comic Detail Page

The detail page presents each comic in a structured, readable layout:

- **Hero section** — cover image (from Open Library), title, author, year, genre, and quick-access buttons (Google Books, Wikipedia, QR code toggle)
- **Description** — publisher description from Google Books or Wikipedia extract
- **AI Summary** — on-demand OpenAI-generated catalogue description (Generate Summary button)
- **Details card** — clean label/value metadata including publisher, page count, subjects, and library classification fields
- **Author bio** — Wikipedia photo and biography when available
- **Related Videos** — up to 5 YouTube video thumbnails (reviews, explainers, reading guides)
- **Related Comics** — up to 6 automatically suggested comics scored by: same author (+3), shared genre or topic (+2 each), year within ±5 years (+1). Displayed as clickable cards.
- **Recently Viewed** — the last 5 unique comics visited in the current session, shown as a quick-navigation strip at the bottom of the page

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
- Recently Viewed — session-based, auto-updated on each comic detail page visit
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

### API Keys

Set keys via `dotnet user-secrets` (never commit them):

```bash
cd FBZSystemMvc
dotnet user-secrets set "OpenAI:ApiKey" "sk-..."
dotnet user-secrets set "YouTube:ApiKey" "AIza..."
dotnet user-secrets set "ComicVine:ApiKey" "your-key"
dotnet user-secrets set "SendGrid:ApiKey" "SG.your-key"
```

On AWS Elastic Beanstalk, set these as environment properties using double-underscore notation (e.g. `OpenAI__ApiKey`).

---

## 🧪 Automated Tests

```bash
dotnet test FBZSystemMvc.Tests/FBZSystemMvc.Tests.csproj
```

22 tests across three suites:

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
│   ├── ExternalApis/     Google Books, Open Library, Comic Vine, Wikipedia, YouTube, OpenAI
│   ├── Persistence/      analytics and saved comics services
│   ├── DatasetUpdates/   background dataset update service
│   ├── RecentlyViewedStore.cs   session-based recently viewed tracking
│   └── SearchListStore.cs       session-based search list
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
| Interface Segregation | Small focused interfaces — `IComicRepository`, `ISearchService`, `IAnalyticsService`, `IYouTubeService`, `IAISummaryService` |
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
