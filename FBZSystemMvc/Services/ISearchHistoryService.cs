using System.Collections.Generic;
using FBZ_System.Domain;

namespace FBZ_System.Services
{
    public interface ISearchHistoryService
    {
        void RecordSearch(SearchResult result);

        IReadOnlyList<(SearchQuery Query, int Count)> GetTopQueries(int top);

        IReadOnlyList<(Comic Comic, int Count)> GetTopResults(int top);

        IReadOnlyList<Comic> GetComicsWithMoreThan(int threshold);

        void Clear();
    }
}
