using System;
using System.Collections.Generic;

namespace FBZ_System.Domain
{
    public class SearchQuery
    {
        // Basic filters
        public string TitleContains { get; set; } = string.Empty;
        public string AuthorContains { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;

        public string Topics { get; set; } = string.Empty;
        
        public string ContentType { get; set; } = string.Empty;

        public string PhysicalDescription { get; set; } = string.Empty;
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }

        // Advanced filters
        public string Language { get; set; } = string.Empty;
        public string Edition { get; set; } = string.Empty;
        public string NameType { get; set; } = string.Empty;

        // Strategy choices
        // e.g. SortBy = "Title", SortDescending = true/false
        public string SortBy { get; set; } = "Title";
        public bool SortDescending { get; set; } = false;

        // e.g. "None", "Author", "Year"
        public string GroupBy { get; set; } = "None";

        // set number of results per page
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// The result of running a query.
    /// </summary>
    public sealed class SearchResult
    {
        public SearchQuery Query { get; }
        public List<Comic> Comics { get; }

        // Parameterless ctor for serializers / designers, keeps the analyser happy
        public SearchResult()
        {
            Query = new SearchQuery();
            Comics = new List<Comic>();
        }

        public SearchResult(SearchQuery query, List<Comic> comics)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
            Comics = comics ?? new List<Comic>();
        }
    }
    }
