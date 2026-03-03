<!-- Copilot instructions for AI coding agents working on this repo -->
# FBZ Encyclopedia — AI Assistant Guidance

Purpose: give AI contributors immediate, actionable context for making safe, correct changes.

- **Big picture:** two front-ends share the same dataset and service logic:
  - WinForms desktop app (root project, Windows-only) for quick UI-driven exploration. See `Form1.cs` and `RecordService.cs`.
  - ASP.NET Core MVC app in `FBZSystemMvc/` for cross-platform web UI. Entrypoint: [FBZSystemMvc/Program.cs](FBZSystemMvc/Program.cs#L1).

- **Data flow & persistence:**
  - CSV dataset files live in `Data/` (`names.csv`, `records.csv`, `titles.csv`). Repositories parse CSVs using Deedle.
  - CSV loader: `Repositories/ComicRepositoryCsv.cs` and `RecordService.cs` (WinForms). These classes assume semicolon-separated multi-values.
  - Web persistence/analytics uses EF Core with SQLite. See `FBZSystemMvc/Persistence/ApplicationDbContext.cs` and `FBZSystemMvc/Migrations/`.

- **Core services and responsibilities:** follow single-responsibility per file.
  - `Services/SearchService.cs` — all filtering, sorting, grouping logic (consumes `IComicRepository`).
  - `Services/ComicFormatter.cs` — string representation / display formatting (used by both UIs).
  - `Services/SearchHistoryService.cs` / `SearchListStore` — history + session list handling.
  - `Strategies/` — plug-in sorting and grouping strategies. Implement `ISortStrategy` / `IGroupingStrategy` to extend behavior.

- **Dependency injection & registrations (how to wire new services):**
  - DI is configured in `FBZSystemMvc/Program.cs`. Examples:
    - Repository registered as `AddSingleton<IComicRepository>` using `ComicRepositoryCsv` with `ContentRootPath + "Data"`.
    - Strategies and services are registered as singletons; analytics/persistence services are scoped. Mirror those lifetimes when adding new components.

- **Concurrency / lifetime notes:**
  - `ComicRepositoryCsv` loads CSV into memory once and is registered as a singleton. Avoid adding per-request state to it.
  - Use `Scoped` for EF Core DB services and anything that should not be shared app-wide.

- **How to run / debug:**
  - Web: `cd FBZSystemMvc && dotnet restore && dotnet run` — Program.cs applies migrations on startup and seeds the `Staff` role.
  - WinForms: open the solution in Visual Studio (Windows), build & run the WinForms project.

- **Common project conventions discovered in code:**
  - Multi-value CSV fields are split by `;` (Repository explicitly preserves commas inside values). See `AddMultiValue` / `AddExtra` in `ComicRepositoryCsv.cs`.
  - Formatter methods return labels/values (e.g., `Genre: [value]`) — keep UI bindings expecting this form.
  - Sorting keys use `Key` strings on strategies (e.g., `TitleAsc`/`TitleDesc`) referenced by `SearchService`.

- **Examples of typical edits an AI might perform:**
  - Add a new sort strategy: implement `ISortStrategy` in `Strategies/`, register with `builder.Services.AddSingleton<ISortStrategy, YourStrategy>()` in `FBZSystemMvc/Program.cs` and root DI where needed.
  - Add a new analytics event: add entity in `FBZSystemMvc/Persistence/Entities`, update `ApplicationDbContext`, add migration (`dotnet ef migrations add Name -p FBZSystemMvc -s FBZSystemMvc`) and let Program.cs migrate on startup.
  - Replace CSV with a DB-backed repository: implement `IComicRepository`, register it in `Program.cs` as `AddSingleton<IComicRepository, YourDbRepo>()`, ensure lifetime matches (avoid storing request data on a singleton).

- **Files to inspect for context when changing behavior:**
  - `Repositories/ComicRepositoryCsv.cs` — CSV parsing rules and value-splitting.
  - `Services/SearchService.cs` — how filters, paging, grouping and sort keys are applied.
  - `Strategies/` — existing strategy patterns and `Key` names.
  - `FBZSystemMvc/Controllers/DatasetController.cs` — how search results are consumed and paged.
  - `FBZSystemMvc/Program.cs` — DI registrations, migrations, and middleware order.
  - `Form1.cs` and `RecordService.cs` — WinForms wiring and UI expectations.

- **External dependencies & tooling:**
  - Deedle for CSV frames, EF Core + SQLite for web persistence, ASP.NET Identity for authentication. Migrations live in `FBZSystemMvc/Migrations/`.

- **Do NOT assume:**
  - There are no unit tests — changes should be minimal and manually tested via the web UI or WinForms where applicable.
  - That CSV splitting uses commas — repository intentionally preserves commas and splits on `;`.

- **Next step if you want more:** request an example PR checklist (e.g., add strategy, register DI, verify UI) or a short code snippet for a new strategy implementation.
