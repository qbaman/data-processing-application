using System.Collections.Generic;
using System.Linq;
using FBZ_System.Domain;

namespace FBZ_System.Strategies
{
    public class SortTitleDescendingStrategy : ISortStrategy //     ONE SORT (Z-A), CAN CREATE NEW SORT BY ADDING NEW CLASS AND NOT EDITING SearchService. OCP
    {
        public string Key => "TitleDesc";

        public IEnumerable<Comic> Sort(IEnumerable<Comic> comics)
        {
            return comics
                .OrderByDescending(c => c.MainTitle)
                .ThenBy(c => c.Id);
        }
    }
}
