namespace FBZSystemMvc.Services.DatasetUpdates;

public class DatasetUpdateOptions
{
    // British Library ZIP
    public string ZipUrl { get; set; } =
        "https://www.bl.uk/bibliographic/downloads/ComicsResearcherFormat_202204_csv.zip";

    public string CsvFileName { get; set; } = "names.csv";

    public int CheckEveryHours { get; set; } = 24;

    public int MinComicsExpected { get; set; } = 1000;
}