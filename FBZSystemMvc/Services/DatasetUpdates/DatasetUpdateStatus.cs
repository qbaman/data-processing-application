using System;

namespace FBZSystemMvc.Services.DatasetUpdates;

public class DatasetUpdateStatus
{
    public string CurrentZipSha256 { get; set; } = "";
    public DateTime? LastCheckedUtc { get; set; }
    public DateTime? LastUpdatedUtc { get; set; }
    public string LastOutcome { get; set; } = "Never";
    public string? LastError { get; set; }

    // runtime info
    public DateTime RepositoryLoadedUtc { get; set; }
    public DateTime? LocalCsvLastWriteUtc { get; set; }
}