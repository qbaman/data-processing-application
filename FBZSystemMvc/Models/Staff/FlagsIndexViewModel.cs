using System;
using System.Collections.Generic;

namespace FBZSystemMvc.Models.Staff;

public class FlagsIndexViewModel
{
    public List<FlagRow> Flags { get; set; } = new();
}

public class FlagRow
{
    public string ComicId { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Reason { get; set; }
    public DateTime CreatedUtc { get; set; }
}