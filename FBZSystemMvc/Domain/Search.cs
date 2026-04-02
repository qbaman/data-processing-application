using System;
using System.Collections.Generic;

namespace FBZ_System.Domain
{
    public class SearchQuery
    {
        private const int MaxFieldLength = 200;

        private string _titleContains = string.Empty;
        private string _authorContains = string.Empty;
        private string _genre = string.Empty;
        private string _resourceType = string.Empty;
        private string _topics = string.Empty;
        private string _contentType = string.Empty;
        private string _physicalDescription = string.Empty;
        private string _language = string.Empty;
        private string _edition = string.Empty;
        private string _nameType = string.Empty;
        private int _pageSize = 50;

        private static string Cap(string value) =>
            value.Length > MaxFieldLength ? value[..MaxFieldLength] : value;

        // Basic filters
        public string TitleContains { get => _titleContains; set => _titleContains = Cap(value ?? string.Empty); }
        public string AuthorContains { get => _authorContains; set => _authorContains = Cap(value ?? string.Empty); }
        public string Genre { get => _genre; set => _genre = Cap(value ?? string.Empty); }
        public string ResourceType { get => _resourceType; set => _resourceType = Cap(value ?? string.Empty); }
        public string Topics { get => _topics; set => _topics = Cap(value ?? string.Empty); }
        public string ContentType { get => _contentType; set => _contentType = Cap(value ?? string.Empty); }
        public string PhysicalDescription { get => _physicalDescription; set => _physicalDescription = Cap(value ?? string.Empty); }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }

        // Advanced filters
        public string Language { get => _language; set => _language = Cap(value ?? string.Empty); }
        public string Edition { get => _edition; set => _edition = Cap(value ?? string.Empty); }
        public string NameType { get => _nameType; set => _nameType = Cap(value ?? string.Empty); }

        // e.g. SortBy = "Title", SortDescending = true/false
        public string SortBy { get; set; } = "Title";
        public bool SortDescending { get; set; } = false;

        // e.g. "None", "Author", "Year"
        public string GroupBy { get; set; } = "None";

        // set number of results per page
        public int Page { get; set; } = 1;
        public int PageSize { get => _pageSize; set => _pageSize = Math.Clamp(value, 1, 100); }
    }

    public sealed class SearchResult
    {
        public SearchQuery Query { get; }
        public List<Comic> Comics { get; }

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
