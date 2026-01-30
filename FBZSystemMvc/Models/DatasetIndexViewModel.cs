using FBZ_System.Domain;

namespace FBZSystemMvc.Models;

public class DatasetIndexViewModel
{
    public SearchQuery Query { get; set; } = new();
    public List<Comic> Results { get; set; } = new();
}
