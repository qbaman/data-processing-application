using System;
using System.Collections.Generic;

namespace FBZSystemMvc.Models.Staff;

public class AnalyticsDashboardViewModel
{
    public List<RecentSearchRow> RecentSearches { get; set; } = new();
    public List<TopQueryRow> TopQueries { get; set; } = new();
    public List<TopResultRow> TopResults { get; set; } = new();
    public List<OverThresholdRow> ComicsOver100 { get; set; } = new();
}

public class RecentSearchRow
{
    public DateTime OccurredUtc { get; set; }
    public string QuerySignature { get; set; } = "";
    public int TotalResults { get; set; }
    public int CountedResults { get; set; }
    public bool Truncated { get; set; }
}

public class TopQueryRow
{
    public string Query { get; set; } = "";
    public int Count { get; set; }
}

public class TopResultRow
{
    public string ComicId { get; set; } = "";
    public string Title { get; set; } = "";
    public int Count { get; set; }
}

public class OverThresholdRow
{
    public string ComicId { get; set; } = "";
    public string Title { get; set; } = "";
    public int Count { get; set; }
}