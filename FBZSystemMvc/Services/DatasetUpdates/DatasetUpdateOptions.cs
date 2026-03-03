namespace FBZSystemMvc.Services.DatasetUpdates;

public class DatasetUpdateOptions
{
    // British Library ZIP (as per brief)
    public string ZipUrl { get; set; } =
        "https://www.bl.uk/bibliographic/downloads/ComicsResearcherFormat_202204_csv.zip";

    // The file inside the ZIP we care about
    public string CsvFileName { get; set; } = "names.csv";

    // Background check interval
    public int CheckEveryHours { get; set; } = 24;

    // Cap for validation (prevents swapping to an empty/broken dataset)
    public int MinComicsExpected { get; set; } = 1000;
}