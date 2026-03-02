using System.Collections.Generic;

namespace FBZ_System
{
    // Small interface that describes what the UI needs from any record service.
    public interface IRecordService
    {
        List<RecordItem> AllRecords { get; }

        void LoadData(string dataFolderPath);

        List<string> GetGenres();
        List<string> GetResourceTypes();

        List<RecordItem> FilterRecords(
            string titlePart,
            string namePart,
            string selectedGenre,
            string selectedType,
            int? yearFrom,
            int? yearTo);

        string BuildStats(IEnumerable<RecordItem> records);
    }
}
