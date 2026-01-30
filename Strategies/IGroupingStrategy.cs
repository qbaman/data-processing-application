using System.Collections.Generic;
using FBZ_System.Domain;

namespace FBZ_System.Strategies
{
    public interface IGroupingStrategy //  HOW GROUPING SHOULD WORK WITHOUT LOCKING IT TO ONE WAY OF GROUPING. OCP
    // SearchServices CAN WORK WITH IGroupingStrategy implementation. Can be swapped without changing hwo the code calls it. LSP 
    {
        string Key { get; }

        IDictionary<string, List<Comic>> Group(IEnumerable<Comic> comics);
    }
}
