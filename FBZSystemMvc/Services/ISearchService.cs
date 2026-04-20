using System.Collections.Generic;
using FBZ_System.Domain;

namespace FBZ_System.Services
{
    public interface ISearchService
    {
        List<Comic> SearchByGenre(SearchQuery query);
        SearchResult AdvancedSearch(SearchQuery query);
        IDictionary<string, List<Comic>> GroupResults(SearchQuery query, List<Comic> comics);
        List<Comic> FindRelated(Comic source, int count = 5);
    }
}

