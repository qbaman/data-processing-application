using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBZ_System.Domain
{
    public class Comic
    {
        public string Id { get; set; } = "";
        public string MainTitle { get; set; } = "";
        public List<string> VariantTitles { get; set; } = new();
        public List<string> Authors { get; set; } = new();
        public List<int> Years { get; set; } = new();
        public List<string> Isbns { get; set; } = new();
        public List<string> Genres { get; set; } = new();
        public List<string> Languages { get; set; } = new();
        public List<string> Editions { get; set; } = new();
        public List<string> NameTypes { get; set; } = new();
        public string NameType { get; set; } = "";
        public Dictionary<string, List<string>> ExtraAttributes { get; set; } = new();
    }
}
