using System;
using System.Collections.Generic;
using System.Linq;
using FBZ_System.Domain;
using FBZ_System.Repositories;
using FBZ_System.Strategies;

namespace FBZ_System.Services
{
    public class SearchService : ISearchService // ONLY HANDLES LOGIC; FILTERING AND SORTING. IT DOESNT KNOW ANYTHING ABOUT THE CSV FILES OR DESIGNER FORM. SINGLE RESPONSIBILITY.
    {
        private readonly IComicRepository _repository;
        private readonly IDictionary<string, IGroupingStrategy> _groupingStrategies;
        private readonly IDictionary<string, ISortStrategy> _sortStrategies;



        public SearchService(
            IComicRepository repository,
            IEnumerable<IGroupingStrategy> groupingStrategies, // SearchService WORKS WITH ARRAY OF STRATEGIES. TO ADD NEW SORTS YOU JUST PASS NEW STRATEGY CLASS INTO THIS ARRAY AND YOU DONT HAVE TO ALTER THE SEARCH LOGIC. OCP
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

            // by title
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

        private List<Comic> FilterBase(SearchQuery query)
        {
            // three focus genres..
            var comics = _repository
            .GetAllComics()
            .ToList();

            // Genre
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

            // Title 
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

            // Author 
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

            //  range
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

            // Lang
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

            // Edition
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

            // Name type

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
