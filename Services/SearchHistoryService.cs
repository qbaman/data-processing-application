using System;
using System.Collections.Generic;
using System.Linq;
using FBZ_System.Domain;

namespace FBZ_System.Services
{
    public class SearchHistoryService : ISearchHistoryService // CLASS ONLY MANAGES SEARC HISTROY, TAKES IN EACH SEARCH AND RETURNS IT AND CLEARS IT, DOESNT DISPLAY ANYTHING. SINGLE RESPONSIBILITY. 
    {
        private readonly Dictionary<string, (SearchQuery Query, int Count)> _queryCounts
            = new(StringComparer.OrdinalIgnoreCase);

        // key = comic Id
        private readonly Dictionary<string, (Comic Comic, int Count)> _resultCounts
            = new(StringComparer.OrdinalIgnoreCase);

        public void RecordSearch(SearchResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            var key = BuildQueryKey(result.Query);

            if (_queryCounts.TryGetValue(key, out var entry))
                _queryCounts[key] = (entry.Query, entry.Count + 1);
            else
                _queryCounts[key] = (result.Query, 1);

            foreach (var comic in result.Comics)
            {
                if (comic == null || string.IsNullOrWhiteSpace(comic.Id))
                    continue;

                if (_resultCounts.TryGetValue(comic.Id, out var existing))
                    _resultCounts[comic.Id] = (existing.Comic, existing.Count + 1);
                else
                    _resultCounts[comic.Id] = (comic, 1);
            }
        }

        public IReadOnlyList<(SearchQuery Query, int Count)> GetTopQueries(int top)
        {
            return _queryCounts.Values
                .OrderByDescending(x => x.Count)
                .ThenBy(q => BuildQueryKey(q.Query))
                .Take(top)
                .ToList();
        }

        public IReadOnlyList<(Comic Comic, int Count)> GetTopResults(int top)
        {
            return _resultCounts.Values
                .OrderByDescending(x => x.Count)
                .ThenBy(c => c.Comic.MainTitle)
                .Take(top)
                .ToList();
        }

        public IReadOnlyList<Comic> GetComicsWithMoreThan(int threshold)
        {
            return _resultCounts.Values
                .Where(x => x.Count > threshold)
                .Select(x => x.Comic)
                .OrderBy(c => c.MainTitle)
                .ToList();
        }

        public void Clear()
        {
            _queryCounts.Clear();
            _resultCounts.Clear();
        }

        private static string BuildQueryKey(SearchQuery q)
        {
            if (q == null) return string.Empty;

            return string.Join("|", new[]
            {
                q.Genre ?? "",
                q.TitleContains ?? "",
                q.AuthorContains ?? "",
                $"{q.YearFrom?.ToString() ?? ""}-{q.YearTo?.ToString() ?? ""}",
                q.Language ?? "",
                q.Edition ?? "",
                q.NameType ?? ""
            });
        }
    }
}
