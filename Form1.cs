using FBZ_System.Domain;
using FBZ_System.Repositories;
using FBZ_System.Services;
using FBZ_System.Strategies;
using FBZEncyclopedia.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FBZ_System
{
    public partial class Form1 : Form // ONLY DEALS WITH USER INTERFACE, SINGLE RESPONSIBILITY
    {
        private readonly IComicRepository _repository;
        private readonly ISearchService _searchService;
        private readonly ISearchHistoryService _history;
        private readonly ComicFormatter _formatter;

        private List<Comic> _currentResults = new();
        private readonly List<Comic> _searchList = new();
        private readonly List<SearchQuery> _historyQueries = new();

        // THIS S WHERE MY CONCRETE CLASSES ARE CREATED. OUTSIDE OF THIS THE UI ONLY DEALS WITH THE INTERFACE. DIP.
        public Form1()
            : this(
                new ComicRepositoryCsv(AppDomain.CurrentDomain.BaseDirectory + "Data"),
                new SearchService(
                    new ComicRepositoryCsv(AppDomain.CurrentDomain.BaseDirectory + "Data"),
                    new IGroupingStrategy[] { new GroupByAuthorStrategy(), new GroupByYearStrategy() },
                    new ISortStrategy[] { new SortTitleAscendingStrategy(), new SortTitleDescendingStrategy() }),
                new SearchHistoryService(),
                new ComicFormatter())
        {
        }

        // FORMS ONLY DEPEND ON SMALL FOCUSED INTERFACE AND FORMATTER. ISP
        // ALSO ONLY DEPENDS ON INTERFACES (IComicRepository, ISearchService) INSTEAD OF CONCRETE CLASSES. DIP
        public Form1( 
            IComicRepository repository,
            ISearchService searchService,
            ISearchHistoryService history,
            ComicFormatter formatter)
        {
            InitializeComponent();

            _repository = repository;
            _searchService = searchService;
            _history = history;
            _formatter = formatter;

            InitialiseCombos();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var query = new SearchQuery
            {
                SortBy = "Title",
                SortDescending = false,
                GroupBy = "None"
            };

            var result = _searchService.AdvancedSearch(query);
            _history.RecordSearch(result);

            _currentResults = result.Comics;
            BindRecordsToGrid(_currentResults);

            txtStats.Text = $"Total results in current view: {_currentResults.Count}";
        }

        private void InitialiseCombos()
        {
            cmbGenre.Items.Clear();
            cmbGenre.Items.Add("Any");
            cmbGenre.Items.Add("Fantasy");
            cmbGenre.Items.Add("Horror");
            cmbGenre.Items.Add("Science fiction");
            cmbGenre.SelectedIndex = 0;

            cmbResourceType.Items.Clear();
            cmbResourceType.Items.Add("Any");
            cmbResourceType.SelectedIndex = 0;

            cmbSortTitle.Items.Clear();
            cmbSortTitle.Items.Add("Title A–Z");
            cmbSortTitle.Items.Add("Title Z–A");
            cmbSortTitle.SelectedIndex = 0;

            cmbGroupBy.Items.Clear();
            cmbGroupBy.Items.Add("None");
            cmbGroupBy.Items.Add("Author");
            cmbGroupBy.Items.Add("Year");
            cmbGroupBy.SelectedIndex = 0;
        }

        private void BindRecordsToGrid(List<Comic> comics)
        {
            _currentResults = comics ?? new List<Comic>();

            var data = _currentResults
                .Select(c => new
                {
                    Title = _formatter.FormatTitle(c),
                    Authors = _formatter.FormatAuthors(c),
                    Years = _formatter.FormatYears(c),
                    Genres = _formatter.FormatGenres(c),
                    Isbn = _formatter.FormatIsbns(c)
                })
                .ToList();

            dgvRecords.DataSource = null;
            dgvRecords.DataSource = data;
        }

        private SearchQuery BuildSearchQueryFromForm()
        {
            var query = new SearchQuery();

            // basic filters
            query.TitleContains = txtTitleFilter.Text.Trim();
            query.AuthorContains = txtNameFilter.Text.Trim();

            string selectedGenre = cmbGenre.SelectedItem as string ?? "Any";
            query.Genre = selectedGenre == "Any" ? string.Empty : selectedGenre;

            string selectedType = cmbResourceType.SelectedItem as string ?? "Any";
            query.ResourceType = selectedType == "Any" ? string.Empty : selectedType;

            // year range
            query.YearFrom = null;
            query.YearTo = null;

            string yearFromText = txtYearFrom.Text.Trim();
            string yearToText = txtYearTo.Text.Trim();

            if (!string.IsNullOrWhiteSpace(yearFromText))
            {
                if (!int.TryParse(yearFromText, out var yf))
                    throw new ApplicationException("Please enter a whole number for 'Year from'.");
                query.YearFrom = yf;
            }

            if (!string.IsNullOrWhiteSpace(yearToText))
            {
                if (!int.TryParse(yearToText, out var yt))
                    throw new ApplicationException("Please enter a whole number for 'Year to'.");
                query.YearTo = yt;
            }

            if (query.YearFrom.HasValue && query.YearTo.HasValue &&
                query.YearFrom.Value > query.YearTo.Value)
            {
                throw new ApplicationException("'Year from' cannot be greater than 'Year to'.");
            }

            // advanced fields
            query.Language = txtLanguage.Text.Trim();
            query.Edition = txtEdition.Text.Trim();
            query.NameType = txtNameType.Text.Trim();

            // sort
            string sortText = cmbSortTitle.SelectedItem as string ?? "Title A–Z";
            query.SortBy = "Title";
            query.SortDescending = sortText == "Title Z–A";

            // group by
            string groupText = cmbGroupBy.SelectedItem as string ?? "None";
            query.GroupBy = groupText;

            return query;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var query = BuildSearchQueryFromForm();
                var result = _searchService.AdvancedSearch(query);

                BindRecordsToGrid(result.Comics);
                _history.RecordSearch(result);

                txtStats.Text = BuildGroupingSummaryText(query, result.Comics);

                _historyQueries.Add(query);
                RefreshHistoryListDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"There was a problem running the search:{Environment.NewLine}{ex.Message}",
                    "Search error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string BuildGroupingSummaryText(SearchQuery query, List<Comic> comics)
        {
            if (comics == null || comics.Count == 0)
                return "No results found for this search.";

            var lines = new List<string>
            {
                $"Total results: {comics.Count}"
            };

            if (!string.Equals(query.GroupBy, "None", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(query.GroupBy, "Author", StringComparison.OrdinalIgnoreCase))
                {
                    var authorGroups = comics
                        .SelectMany(c => c.Authors ?? Enumerable.Empty<string>())
                        .Select(a => string.IsNullOrWhiteSpace(a) ? "Unknown" : a.Trim())
                        .GroupBy(a => a)
                        .OrderBy(g => g.Key);

                    lines.Add("Grouped by Author:");
                    foreach (var g in authorGroups)
                    {
                        lines.Add($" - {g.Key}: {g.Count()} items");
                    }
                }
                else if (string.Equals(query.GroupBy, "Year", StringComparison.OrdinalIgnoreCase))
                {
                    var yearGroups = comics
                        .Select(c => (c.Years != null && c.Years.Any()) ? c.Years.First() : 0)
                        .GroupBy(y => y)
                        .OrderBy(g => g.Key);

                    lines.Add("Grouped by Year:");
                    foreach (var y in yearGroups)
                    {
                        string label = y.Key == 0 ? "Unknown" : y.Key.ToString();
                        lines.Add($" - {label}: {y.Count()} items");
                    }
                }
            }

            return string.Join(Environment.NewLine, lines);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtTitleFilter.Clear();
            txtNameFilter.Clear();
            txtYearFrom.Clear();
            txtYearTo.Clear();
            txtLanguage.Clear();
            txtEdition.Clear();
            txtNameType.Clear();

            if (cmbGenre.Items.Count > 0)
                cmbGenre.SelectedIndex = 0;
            if (cmbResourceType.Items.Count > 0)
                cmbResourceType.SelectedIndex = 0;
            if (cmbSortTitle.Items.Count > 0)
                cmbSortTitle.SelectedIndex = 0;
            if (cmbGroupBy.Items.Count > 0)
                cmbGroupBy.SelectedIndex = 0;

            _history.Clear();
            _historyQueries.Clear();
            _searchList.Clear();
            _currentResults.Clear();

            RefreshSearchListDisplay();
            RefreshHistoryListDisplay();

            dgvRecords.DataSource = null;
            txtStats.Clear();
        }

        private void btnShowStats_Click(object sender, EventArgs e)
        {
            if (_currentResults == null || _currentResults.Count == 0)
            {
                txtStats.Text = "No results loaded. Run a search first.";
                return;
            }

            var lines = new List<string>();

            lines.Add($"Statistics for current view ({_currentResults.Count} items)");
            lines.Add(string.Empty);

            // by genre
            var genreGroups = _currentResults
                .SelectMany(c => c.Genres ?? Enumerable.Empty<string>())
                .Select(g => string.IsNullOrWhiteSpace(g) ? "Unknown" : g.Trim())
                .GroupBy(g => g)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key);

            lines.Add("By genre:");
            foreach (var g in genreGroups)
            {
                lines.Add($" - {g.Key}: {g.Count()} items");
            }

            // by year
            var yearGroups = _currentResults
                .Select(c => (c.Years != null && c.Years.Any()) ? c.Years.First() : 0)
                .GroupBy(y => y)
                .OrderBy(g => g.Key);

            lines.Add(string.Empty);
            lines.Add("By year:");
            foreach (var y in yearGroups)
            {
                string label = y.Key == 0 ? "Unknown" : y.Key.ToString();
                lines.Add($" - {label}: {y.Count()} items");
            }

            // top authors
            var authorGroups = _currentResults
                .SelectMany(c => c.Authors ?? Enumerable.Empty<string>())
                .Select(a => string.IsNullOrWhiteSpace(a) ? "Unknown" : a.Trim())
                .GroupBy(a => a)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .Take(10);

            lines.Add(string.Empty);
            lines.Add("Top authors (up to 10):");
            foreach (var a in authorGroups)
            {
                lines.Add($" - {a.Key}: {a.Count()} items");
            }

            txtStats.Text = string.Join(Environment.NewLine, lines);
        }

        // ---------- SEARCH LIST (SESSION) ----------

        private void btnAddToSearchList_Click(object sender, EventArgs e)
        {
            if (dgvRecords.CurrentRow == null)
                return;

            var rowIndex = dgvRecords.CurrentRow.Index;
            if (rowIndex < 0 || rowIndex >= _currentResults.Count)
                return;

            var comic = _currentResults[rowIndex];

            if (!_searchList.Any(c => c.Id == comic.Id))
            {
                _searchList.Add(comic);
                RefreshSearchListDisplay();
            }
        }

        private void btnRemoveFromSearchList_Click(object sender, EventArgs e)
        {
            if (lstSearchList.SelectedItem is Comic comic)
            {
                _searchList.RemoveAll(c => c.Id == comic.Id);
                RefreshSearchListDisplay();
            }
        }

        private void RefreshSearchListDisplay()
        {
            lstSearchList.DataSource = null;
            lstSearchList.DataSource = _searchList;
            lstSearchList.DisplayMember = "MainTitle";
        }

        private void btnShowListOnly_Click(object sender, EventArgs e)
        {
            if (_searchList.Count == 0)
            {
                MessageBox.Show(
                    "The search list is currently empty.",
                    "Search list",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            BindRecordsToGrid(_searchList);
            txtStats.Text = $"Showing search list only ({_searchList.Count} items)";
        }

        private void btnShowAllRecords_Click(object sender, EventArgs e)
        {
            var all = _repository.GetAllComics().ToList();
            BindRecordsToGrid(all);
            txtStats.Text = $"Showing all records ({all.Count} items)";
        }

        private void btnClearSearchList_Click(object sender, EventArgs e)
        {
            _searchList.Clear();
            RefreshSearchListDisplay();
            txtStats.Text = "Search list cleared.";
        }

        // ---------- SEARCH HISTORY UI ----------

        private void RefreshHistoryListDisplay()
        {
            var items = _historyQueries
                .Select((q, index) => new HistoryItem
                {
                    Query = q,
                    Display = $"{index + 1}. {Shorten(q.TitleContains)} | {Shorten(q.AuthorContains)} | {q.Genre} | {q.YearFrom}-{q.YearTo}"
                })
                .ToList();

            lstHistory.DataSource = null;
            lstHistory.DataSource = items;
            lstHistory.DisplayMember = "Display";
        }

        private string Shorten(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "(any)";
            return s.Length > 20 ? s.Substring(0, 20) + "…" : s;
        }

        private void btnShowHistory_Click(object sender, EventArgs e)
        {
            if (_historyQueries.Count == 0)
            {
                txtStats.Text = "No searches have been run yet.";
                lstHistory.DataSource = null;
                return;
            }

            RefreshHistoryListDisplay();
            txtStats.Text = $"History: {_historyQueries.Count} searches stored.";
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            _historyQueries.Clear();
            _history.Clear();

            lstHistory.DataSource = null;
            txtStats.Text = "Search history cleared.";
        }

        private void lstHistory_DoubleClick(object sender, EventArgs e)
        {
            if (lstHistory.SelectedItem is not HistoryItem item || item.Query == null)
                return;

            var result = _searchService.AdvancedSearch(item.Query);

            BindRecordsToGrid(result.Comics);
            txtStats.Text = BuildGroupingSummaryText(item.Query, result.Comics);

            _history.RecordSearch(result);
        }

        private class HistoryItem
        {
            public SearchQuery? Query { get; set; }
            public string? Display { get; set; }
        }
    }
}
