using System.Collections.Generic;
using FBZ_System.Domain;

namespace FBZSystemMvc.Models;

public class ComicDetailsViewModel
{
    public Comic Comic { get; set; } = new();

    public List<string> IsbnsDisplay { get; set; } = new();
    public List<int> YearsDisplay { get; set; } = new();
    public List<string> AuthorsDisplay { get; set; } = new();
    public List<string> GenresDisplay { get; set; } = new();

    // Optional: show extra fields as key:value lines if you have them
    public List<string> InfoLines { get; set; } = new();
}
