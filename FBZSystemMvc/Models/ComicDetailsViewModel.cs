using System.Collections.Generic;
using FBZ_System.Domain;
using FBZSystemMvc.Services.ExternalApis;

namespace FBZSystemMvc.Models;

public class ComicDetailsViewModel
{
    public Comic Comic { get; set; } = new();

    public List<string> IsbnsDisplay { get; set; } = new();
    public List<int> YearsDisplay { get; set; } = new();
    public List<string> AuthorsDisplay { get; set; } = new();
    public List<string> GenresDisplay { get; set; } = new();

    public List<string> InfoLines { get; set; } = new();

    public bool IsFlagged { get; set; }
    public string? FlagReason { get; set; }
    public GoogleBooksLookupResult? GoogleBooks { get; set; }
    public string? OpenLibraryCoverUrl { get; set; }
    public OpenLibraryLookupResult? OpenLibrary { get; set; }
    public ComicVineLookupResult? ComicVine { get; set; }
    public WikipediaLookupResult? Wikipedia { get; set; }
    public YouTubeLookupResult? YouTube { get; set; }

    public List<Comic> RelatedComics { get; set; } = new();
    public List<Comic> RecentlyViewedComics { get; set; } = new();
}