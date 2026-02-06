using System.Collections.Generic;
using FBZ_System.Domain;

namespace FBZSystemMvc.Models;

public class DatasetIndexViewModel
{
    public SearchQuery Query { get; set; } = new();

    public List<Comic> Results { get; set; } = new();

    public List<Comic> SearchList { get; set; } = new();

    public int TotalResults { get; set; }
    public int TotalPages { get; set; }

    public List<string> AllGenres { get; set; } = new();

    public List<string> AllLanguages { get; set; } = new();


    public List<string> AllNameTypes { get; set; } = new();

    public IDictionary<string, List<Comic>> GroupedResults { get; set; }
        = new Dictionary<string, List<Comic>>();
}
