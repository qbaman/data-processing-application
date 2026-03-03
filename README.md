# FBZ Encyclopedia (Fantasy B’zaar Encyclopedia System)

A C# project for loading, searching, filtering, sorting, grouping, and displaying comic data from the British Library **“Comics Unmasked”** dataset (Phase 1 focus: `names.csv`).

This repo contains:
- **WinForms app (Windows-only)** — the original FBZSystem desktop application.
- **ASP.NET Core MVC app (cross-platform)** — the current web-based version in **`/FBZSystemMvc`**.

---

## 📘 Overview
FBZ Encyclopedia provides a lightweight way to explore a large comic dataset (70,000+ rows in `names.csv`) and supports both public customers and staff users.

The MVC application implements:
- Fast dataset search and advanced filtering
- Role-based access (Public vs Staff)
- Persistent user features (saved lists)
- Staff tools (flags + analytics)
- Desktop + kiosk-friendly UI

---

## 🧾 Dataset & Licensing
Dataset: British Library “Comics Unmasked” (Researcher Format).  
License: **CC0 1.0 Universal (Public Domain)** — no restrictions on use/reuse.  
Note: The British Library **does not endorse** applications created using the dataset.

Dataset sources:
- https://www.bl.uk/collection-metadata/downloads
- https://www.bl.uk/bibliographic/downloads/ComicsResearcherFormat_202204_csv.zip

---

## 🔎 Key Features

### ASP.NET Core MVC (Current)
Located in: **`/FBZSystemMvc`**

**Search & display**
- Loads `names.csv` into memory for fast searching
- Handles special characters and formats fields into readable output
- Combines duplicate rows into a single comic record with variant values (e.g., years/ISBNs as lists)
- Displays `"missing"` when ISBN is not present

**Search, filtering, grouping**
- Basic search: title contains, author contains
- Advanced search parameters:
  - Genre, name type
  - Publication year range
  - Language, edition
  - Resource type, content type, physical description, topics
- Sorting: title **A–Z** and **Z–A**
- Grouping: **None / Author / Year**
- Paging + configurable page size (prevents huge tables)

**Session Search List**
- Add / Remove / Clear
- Export Search List to CSV
- Finish/Exit clears the session list

**Accounts & persistence**
- ASP.NET Core Identity (register/login/logout)
- Persistent **Saved Lists** (saved search results) stored in SQLite and available across sessions/devices

**Staff-only features**
- Flag records for review (with reason)
- View flagged records list
- Real-time analytics dashboard:
  - Top 10 most common search queries
  - Top 10 most frequently returned results
  - Comics included in more than 100 searches (populates over time)

**Kiosk mode**
- Kiosk UI mode via `?kiosk=1` with touch-friendly sizing
- Kiosk state preserved during Add/Remove/Clear actions
- Toggle back to normal desktop mode

**Dataset updates (Phase 1)**
- Staff dataset update page (safe update attempt + status)
- Background checker framework in place
- Update failures handled gracefully (app remains usable)

---

### WinForms (Original)
- CSV data loading using a repository class (`ComicRepositoryCsv`)
- Search functionality with multiple filters
- Sorting and grouping via strategy classes
- Search history / list management
- Consistent display output using formatter class
- Separated UI, logic, and data layers (SOLID-aligned)

---

## ▶️ Run Locally (ASP.NET Core MVC)

### Prerequisites
- .NET SDK installed (`dotnet --version`)

### Steps
```bash
cd FBZSystemMvc
dotnet restore
dotnet run
````

Open the URL shown in the terminal (usually `http://localhost:####`) and use:

* `/` (Home / introduction)
* `/Dataset` (Search)
* `/Dataset?kiosk=1` (Kiosk mode)

Staff tools (requires Staff role):

* `/staff/flags`
* `/staff/analytics`
* `/staff/dataset`

---

## 🧪 Automated Tests

A test project is included to provide automated evidence of correctness.

Run:

```bash
dotnet test FBZSystemMvc.Tests/FBZSystemMvc.Tests.csproj
```

Includes:

* Smoke tests for key routes (Home/Dataset/Kiosk)
* Unit tests for AnalyticsService using an in-memory SQLite database

---

## 🪟 Run (WinForms - Windows Only)

The original WinForms app targets Windows. To run:

* Open the solution on Windows (Visual Studio)
* Build and run the WinForms project

---

## 📁 Project Structure

* `Data/` — CSV dataset files (Phase 1 uses `names.csv`)
* `Domain/` — domain models / core types
* `Repositories/` — data access (CSV repository)
* `Services/` — search, analytics, dataset updates, saved lists, orchestration logic
* `Strategies/` — sorting and grouping strategies
* `FBZSystemMvc/` — ASP.NET Core MVC web app (cross-platform)
* `FBZSystemMvc.Tests/` — automated tests (xUnit)

---

## 🧩 SOLID Principles Guide

This project contains examples of all five SOLID principles:

**Single Responsibility**

* `ComicRepositoryCsv` — loads/parses CSV
* `SearchService` — search/filter/sort/group logic
* `AnalyticsService` — analytics aggregation logic
* Dataset update service — update orchestration + safe swap
* MVC controllers/views — presentation only

**Open/Closed**

* Sort strategies (ascending/descending)
* Group strategies (author/year)
* Extend by adding new strategies without changing core search logic

**Liskov Substitution**

* Strategy interfaces can be swapped safely
* Repository abstraction supports future storage changes

**Interface Segregation**

* Small focused interfaces (repository/services)

**Dependency Inversion**

* Controllers depend on abstractions
* Services depend on interfaces
* DI wiring in one place

---

## 🛠️ Technologies Used

* C# (.NET)
* ASP.NET Core MVC + Identity
* EF Core + SQLite
* CSV dataset processing + LINQ
* Strategy pattern (sorting/grouping)
* xUnit automated tests

---

## 👤 Author

**Aman — HND Cloud & AI Computing**
