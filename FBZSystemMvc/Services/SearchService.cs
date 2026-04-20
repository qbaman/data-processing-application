using System;
using System.Collections.Generic;
using System.Linq;
using FBZ_System.Domain;
using FBZ_System.Repositories;
using FBZ_System.Strategies;

namespace FBZ_System.Services
{
    public class SearchService : ISearchService 
    {
        private readonly IComicRepository _repository;
        private readonly IDictionary<string, IGroupingStrategy> _groupingStrategies;
        private readonly IDictionary<string, ISortStrategy> _sortStrategies;



        public SearchService(
            IComicRepository repository,
            IEnumerable<IGroupingStrategy> groupingStrategies, 
            IEnumerable<ISortStrategy> sortStrategies)
        {
            _repository = repository;

            _groupingStrategies = groupingStrategies
                .ToDictionary(g => g.Key, g => g, StringComparer.OrdinalIgnoreCase);

            _sortStrategies = sortStrategies
                .ToDictionary(s => s.Key, s => s, StringComparer.OrdinalIgnoreCase);
        }

        public SearchResult AdvancedSearch(SearchQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var comics = FilterBase(query);

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                var sortKey = query.SortDescending ? "TitleDesc" : "TitleAsc";

                if (_sortStrategies.TryGetValue(sortKey, out var sorter))
                {
                    comics = sorter.Sort(comics).ToList();
                }
            }

            return new SearchResult(query, comics);
        }

        public List<Comic> SearchByGenre(SearchQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var genreOnly = new SearchQuery
            {
                Genre = query.Genre
            };

            return FilterBase(genreOnly);
        }

        public IDictionary<string, List<Comic>> GroupResults(SearchQuery query, List<Comic> comics)
        {
            var key = (query.GroupBy ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(key) ||
                string.Equals(key, "None", StringComparison.OrdinalIgnoreCase))
            {
                return new Dictionary<string, List<Comic>>();
            }

            if (_groupingStrategies.TryGetValue(key, out var strategy))
            {
                return strategy.Group(comics);
            }

            return new Dictionary<string, List<Comic>>();
        }

        public List<Comic> FindRelated(Comic source, int count = 5)
        {
            if (source is null) return new List<Comic>();

            var sourceAuthors = new HashSet<string>(
                (source.Authors ?? Enumerable.Empty<string>())
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a!.Trim().ToLowerInvariant()),
                StringComparer.OrdinalIgnoreCase);

            var sourceGenres = new HashSet<string>(
                (source.Genres ?? Enumerable.Empty<string>())
                    .Where(g => !string.IsNullOrWhiteSpace(g))
                    .Select(g => g!.Trim().ToLowerInvariant()),
                StringComparer.OrdinalIgnoreCase);

            var sourceTopics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (source.ExtraAttributes?.TryGetValue("Topics", out var srcTopics) == true && srcTopics is not null)
                foreach (var t in srcTopics.Where(t => !string.IsNullOrWhiteSpace(t)))
                    sourceTopics.Add(t!.Trim().ToLowerInvariant());

            var sourceYear = source.Years?.Count > 0 ? source.Years.Min() : (int?)null;

            return _repository.GetAllComics()
                .Where(c => c.Id != source.Id)
                .Select(c =>
                {
                    int score = 0;

                    if (c.Authors is not null &&
                        c.Authors.Any(a => !string.IsNullOrWhiteSpace(a) && sourceAuthors.Contains(a.Trim())))
                        score += 3;

                    if (c.Genres is not null)
                        score += c.Genres.Count(g => !string.IsNullOrWhiteSpace(g) && sourceGenres.Contains(g.Trim())) * 2;

                    if (c.ExtraAttributes?.TryGetValue("Topics", out var cTopics) == true && cTopics is not null)
                        score += cTopics.Count(t => !string.IsNullOrWhiteSpace(t) && sourceTopics.Contains(t.Trim())) * 2;

                    if (sourceYear.HasValue && c.Years?.Count > 0 &&
                        Math.Abs(c.Years.Min() - sourceYear.Value) <= 5)
                        score += 1;

                    return (Comic: c, Score: score);
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => x.Comic)
                .ToList();
        }

        private List<Comic> FilterBase(SearchQuery query)
        {
            var comics = _repository
            .GetAllComics()
            .ToList();

            if (!string.IsNullOrWhiteSpace(query.Genre))
            {
                var term = query.Genre.Trim();

                comics = comics
                    .Where(c => c.Genres != null &&
                                c.Genres.Any(g =>
                                    g != null &&
                                    g.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.PhysicalDescription))
            {
                var wanted = query.PhysicalDescription.Trim();

                comics = comics.Where(c =>
                    c.ExtraAttributes != null &&
                    c.ExtraAttributes.TryGetValue("Physical description", out var vals) &&
                    vals != null &&
                    vals.Any(v => string.Equals((v ?? "").Trim(), wanted, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.ResourceType))
            {
                var wanted = query.ResourceType.Trim();

                comics = comics.Where(c =>
                    c.ExtraAttributes != null &&
                    c.ExtraAttributes.TryGetValue("Type of resource", out var vals) &&
                    vals != null &&
                    vals.Any(v =>
                        string.Equals((v ?? "").Trim(), wanted, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.Topics))
            {
                var wanted = query.Topics.Trim();

                comics = comics.Where(c =>
                    c.ExtraAttributes != null &&
                    c.ExtraAttributes.TryGetValue("Topics", out var vals) &&
                    vals != null &&
                    vals.Any(v => string.Equals((v ?? "").Trim(), wanted, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.ContentType))
            {
                var wanted = query.ContentType.Trim();
                comics = comics.Where(c =>
                    c.ExtraAttributes != null &&
                    c.ExtraAttributes.TryGetValue("Content type", out var vals) &&
                    vals != null &&
                    vals.Any(v => string.Equals((v ?? "").Trim(), wanted, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.TitleContains))
            {
                var term = query.TitleContains.Trim();

                comics = comics
                    .Where(c =>
                        (!string.IsNullOrWhiteSpace(c.MainTitle) &&
                         c.MainTitle.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                        ||
                        (c.VariantTitles != null &&
                         c.VariantTitles.Any(t =>
                             t != null &&
                             t.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.AuthorContains))
            {
                var term = query.AuthorContains.Trim();

                comics = comics
                    .Where(c => c.Authors != null &&
                                c.Authors.Any(a =>
                                    a != null &&
                                    a.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            if (query.YearFrom.HasValue)
            {
                comics = comics
                    .Where(c => c.Years != null &&
                                c.Years.Any(y => y >= query.YearFrom.Value))
                    .ToList();
            }

            if (query.YearTo.HasValue)
            {
                comics = comics
                    .Where(c => c.Years != null &&
                                c.Years.Any(y => y <= query.YearTo.Value))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.Language))
            {
                var term = query.Language.Trim();

                comics = comics
                    .Where(c => c.Languages != null &&
                                c.Languages.Any(l =>
                                    l != null &&
                                    l.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.Edition))
            {
                var term = query.Edition.Trim();

                comics = comics
                    .Where(c => c.Editions != null &&
                                c.Editions.Any(e =>
                                    e != null &&
                                    e.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.NameType))
            {
                var term = query.NameType.Trim();

                comics = comics
                    .Where(c => c.NameTypes != null &&
                                c.NameTypes.Any(nt =>
                                    nt != null &&
                                    nt.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            return comics;
        }
    }
}
