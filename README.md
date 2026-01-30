# FBZ Encyclopedia  
A C# WinForms application for loading, searching, filtering, sorting, grouping, and displaying comic data from a CSV dataset. The project is designed around modular services and interfaces, with clear use of SOLID principles throughout the codebase.

---

## 📘 Overview
FBZ Encyclopedia provides a lightweight and responsive interface for exploring a comic dataset.  
All data operations—loading, searching, formatting, and history management—are separated into dedicated classes for maintainability and clarity.  

The UI remains clean and simple, while all logic is handled by services and strategy patterns behind the scenes.

---

## 🔎 Key Features
- CSV data loading using a repository class (`ComicRepositoryCsv`)
- Powerful search functionality with multiple filters
- Sorting and grouping via plug-in strategy classes
- Search history tracking with record/clear functionality
- Consistent display output using a formatter class
- Completely separated UI, logic, and data layers
- Full implementation of SOLID principles across services and interfaces

---

## 🧩 SOLID Principles Guide
This project includes multiple examples of all five SOLID principles.  
Each example is marked directly in the code using `//` comments.

### **S — Single Responsibility Principle**
- `ComicRepositoryCsv` — loads and parses CSV data  
- `SearchService` — search, filter, sort, group logic  
- `SearchHistoryService` — manages search history  
- `ComicFormatter` — creates display strings  
- `Form1` — purely UI behaviour  

### **O — Open/Closed Principle**
- Sorting strategies (`SortTitleAscendingStrategy`, `SortTitleDescendingStrategy`)  
- Grouping strategies (`GroupByAuthorStrategy`, `GroupByYearStrategy`)  
- New strategies can be added at any time without editing `SearchService`.

### **L — Liskov Substitution Principle**
- Any `ISortStrategy` works in the search engine  
- Any `IGroupingStrategy` works with the same calls  
- The `IComicRepository` could be replaced (CSV → SQL → API)  

### **I — Interface Segregation Principle**
- `IComicRepository`  
- `ISearchService`  
- `ISearchHistoryService`  

### **D — Dependency Inversion Principle**
- `Form1` receives interfaces (`IComicRepository`, `ISearchService`, `ISearchHistoryService`)  
- `SearchService` depends on arrays of strategy interfaces, not their concrete types  
- The composition root wires everything together in one place  

---

## 🛠️ Technologies Used
- C# (.NET)  
- WinForms  
- CSV datasets  
- LINQ  
- Strategy pattern for sorting and grouping
- Deedle

---

## ⚙️ How the System Works
- The CSV repository loads the entire dataset.  
- The search service handles filtering, sorting, and grouping through injected strategies.  
- The search history service records all previous queries.  
- The formatter prepares clean display strings for the UI.  
- The form presents results and interacts with the services.

---

## 👤 Author  
**Aman Singh – HND Cloud & AI Computing**
