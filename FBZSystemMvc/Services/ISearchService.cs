using System.Collections.Generic;
using FBZ_System.Domain;

namespace FBZ_System.Services
{
    public interface ISearchService // ONLY CONTAINS SEARCH-RELATED ACTIONOS. OTHER THINGS LIKE HISTORY AND FORMATTING ARE IN SEPERATE CLASSES. ISP
    {
        List<Comic> SearchByGenre(SearchQuery query);
        SearchResult AdvancedSearch(SearchQuery query);

        IDictionary<string, List<Comic>> GroupResults(SearchQuery query, List<Comic> comics);
    }
}

