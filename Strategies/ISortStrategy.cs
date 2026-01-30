using System.Collections.Generic;
using FBZ_System.Domain;

namespace FBZ_System.Strategies
{
    public interface ISortStrategy // ANYWHERE CODE CALLS ISortStrategy, IT CAN USE ANY OF THE SORTING CLASSES, LSP
    {
        string Key { get; }

        IEnumerable<Comic> Sort(IEnumerable<Comic> comics);
    }
}
