using System.Collections.Generic;
using System.Linq;
using FBZ_System.Domain;

namespace FBZ_System.Strategies
{
    public class GroupByAuthorStrategy : IGroupingStrategy
    {
        public string Key => "Author";

        public IDictionary<string, List<Comic>> Group(IEnumerable<Comic> comics)
        {
            return comics
                .SelectMany(c => c.Authors.DefaultIfEmpty("Unknown"))
                .GroupBy(a => a)
                .ToDictionary(
                    g => g.Key,
                    g => comics.Where(c => c.Authors.Contains(g.Key)).ToList());
        }
    }
}
