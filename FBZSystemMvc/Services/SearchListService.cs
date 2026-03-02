using FBZ_System.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBZEncyclopedia.Services
{
    public class SearchListService
    {
        private readonly List<Comic> _list = new();

        public void Add(Comic comic)
        {
            if (comic == null) return;
            if (!_list.Contains(comic))
                _list.Add(comic);
        }

        public void Remove(Comic comic)
        {
            if (comic == null) return;
            _list.Remove(comic);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public IReadOnlyList<Comic> GetAll()
        {
            return _list;
        }
    }
}
