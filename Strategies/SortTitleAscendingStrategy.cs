using System.Collections.Generic;
using System.Linq;
using FBZ_System.Domain;

namespace FBZ_System.Strategies
{
    public class SortTitleAscendingStrategy : ISortStrategy // ONLY ONE SORT (A-Z), NEW SORTS CAN BE ADDED WITH NEW CLASSES INSTEAD OF CHANGING CODE. OCP
    {
        public string Key => "TitleAsc";

        public IEnumerable<Comic> Sort(IEnumerable<Comic> comics)
        {
            return comics
                .OrderBy(c => c.MainTitle)
                .ThenBy(c => c.Id);
        }
    }
}
