using System.Collections.Generic;
using System.Linq;
using FBZ_System.Domain;

namespace FBZ_System.Strategies
{
    public class GroupByYearStrategy : IGroupingStrategy
    {
        public string Key => "Year";

        public IDictionary<string, List<Comic>> Group(IEnumerable<Comic> comics)
        {
            return comics
                .SelectMany(c => c.Years.DefaultIfEmpty(0), (c, y) => new { Comic = c, Year = y })
                .GroupBy(x => x.Year == 0 ? "Unknown year" : x.Year.ToString())
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Comic).Distinct().ToList());
        }
    }
}
