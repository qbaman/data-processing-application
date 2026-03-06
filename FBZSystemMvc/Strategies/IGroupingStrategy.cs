using System.Collections.Generic;
using FBZ_System.Domain;

namespace FBZ_System.Strategies
{
    public interface IGroupingStrategy // ocp
    {
        string Key { get; }

        IDictionary<string, List<Comic>> Group(IEnumerable<Comic> comics);
    }
}
