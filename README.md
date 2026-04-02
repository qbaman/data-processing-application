# FBZ Encyclopedia
### Fantasy B'zaar Encyclopedia System

> A full-stack ASP.NET Core MVC web application for exploring the British Library **Comics Unmasked** dataset — 70,000+ comic records with search, filtering, enrichment from multiple external APIs, and a full CI/CD pipeline to AWS Elastic Beanstalk.

---

## ✨ Features at a Glance

| Area | Highlights |
|------|-----------|
| 🔍 **Search** | Title, author, genre, year range, language, edition, resource type, content type |
| 📚 **External APIs** | Google Books · Open Library · Comic Vine · QR Code |
| 👤 **Accounts** | Register / Login / Logout via ASP.NET Core Identity |
| 💾 **Saved Lists** | Persist search results across sessions in SQLite |
| 🛡️ **Staff Tools** | Flag records, analytics dashboard, dataset updates |
| 🖥️ **Kiosk Mode** | Touch-friendly UI for public terminals |
| 🚀 **CI/CD** | GitHub Actions → AWS Elastic Beanstalk (auto-deploy on push to `main`) |

---

## 🌐 External API Integrations

Each comic detail page is enriched with live data from four external APIs:

### 📖 Google Books
Matches the comic by ISBN/title and pulls publisher, page count, description, and preview links.

### 🏛️ Open Library
Fetches cover art, first publish year, subjects, and related ISBNs from the Internet Archive's Open Library.

### 🦸 Comic Vine
Looks up comic-specific metadata including publisher, deck summary, and series information.

### 📱 QR Code *(api.qrserver.com)*
Generates a scannable QR code for every detail page on-the-fly — no API key required. Scan it with a phone to instantly open the same comic.

---

## 🔎 Search & Display

- Loads `names.csv` (~70K rows) into memory for fast in-process search
- Combines duplicate rows into single records with variant values (years, ISBNs, authors as lists)
- Sorting: **A–Z** / **Z–A** via pluggable strategy classes
- Grouping: **None / Author / Year**
- Paging with configurable page size

---

## 👤 Accounts & Persistence

- **ASP.NET Core Identity** — register, login, logout with hashed passwords
- **Saved Lists** — save search results to SQLite, accessible across devices and sessions
- **Session List** — temporary add/remove/clear/export-to-CSV within a session
- **Kiosk mode** — `?kiosk=1` activates a touch-optimised layout; Finish/Exit clears the session

---

## 🛡️ Staff Features

Staff accounts (role-gated) have access to:

| Route | Purpose |
|-------|---------|
| `/staff/flags` | Flag records for review with a reason; unflag when resolved |
| `/staff/analytics` | Top 10 queries, top 10 results, comics appearing in 100+ searches |
| `/staff/dataset` | Trigger a live dataset update from the British Library source URL |

---

## 🚀 CI/CD Pipeline

Every push to `main` triggers a fully automated pipeline:

```
Push to main
    → GitHub Actions: Restore → Build → Test
    → dotnet publish
    → Copy names.csv + Procfile into artefact
    → Verify artefact (fast-fail if files missing)
    → Zip → Upload to S3
    → Create Elastic Beanstalk application version
    → Deploy to EB environment
```

Infrastructure is defined in `infra/terraform/` (AWS provider, us-east-1, t3.micro, .NET 9 on Amazon Linux 2023).

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
| `/staff/flags` | Staff — flagged records |
| `/staff/analytics` | Staff — analytics dashboard |
| `/staff/dataset` | Staff — dataset update |

---

## 🧪 Automated Tests

```bash
dotnet test FBZSystemMvc.Tests/FBZSystemMvc.Tests.csproj
```

- **Smoke tests** — verifies key routes return 200 (Home, Dataset, Kiosk)
- **Unit tests** — `AnalyticsService` logic tested against an in-memory SQLite database

---

## 📁 Project Structure

```
FBZSystemMvc/
├── Controllers/          # MVC controllers (Staff routes under Controllers/Staff/)
├── Data/                 # names.csv dataset
├── Domain/               # Core domain types (Comic, Search)
├── Migrations/           # EF Core migrations
├── Models/               # ViewModels
├── Persistence/          # EF Core DbContext + entities
├── Repositories/         # CSV data access + hot-reload support
├── Services/
│   ├── ExternalApis/     # GoogleBooks, OpenLibrary, ComicVine, QR Code
│   ├── Persistence/      # Analytics, SavedComics services
│   └── DatasetUpdates/   # Background dataset update service
├── Strategies/           # Sort + group strategies (Strategy pattern)
├── Views/                # Razor views
└── wwwroot/              # Static assets
infra/terraform/          # AWS infrastructure definitions
.github/workflows/        # CI/CD pipeline
```

---

## 🧩 SOLID Principles

| Principle | Example |
|-----------|---------|
| **Single Responsibility** | `ComicRepositoryCsv` loads CSV · `SearchService` handles search logic · `AnalyticsService` handles analytics |
| **Open/Closed** | New sort/group strategies can be added without changing `SearchService` |
| **Liskov Substitution** | Any `ISortStrategy` or `IGroupingStrategy` can be swapped in transparently |
| **Interface Segregation** | Small focused interfaces — `IComicRepository`, `ISearchService`, `IAnalyticsService` |
| **Dependency Inversion** | Controllers and services depend on abstractions; all wiring in `Program.cs` |

---

## 🛠️ Tech Stack

- **C# / .NET 9** — ASP.NET Core MVC
- **EF Core + SQLite** — persistence and migrations
- **ASP.NET Core Identity** — authentication and role management
- **Deedle** — CSV dataset loading
- **xUnit** — automated testing
- **GitHub Actions** — CI/CD
- **AWS Elastic Beanstalk + S3** — cloud hosting
- **Terraform** — infrastructure as code

---

## 📋 Dataset

**British Library — Comics Unmasked** (Researcher Format)
License: **CC0 1.0 Universal (Public Domain)**
Source: [bl.uk/collection-metadata/downloads](https://www.bl.uk/collection-metadata/downloads)

> The British Library does not endorse this application.

---

## 👤 Author

**Aman** — HND Cloud & AI Computing
