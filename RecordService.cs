using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Deedle;

// This class handles all data:

namespace FBZ_System
{
    public class RecordService : IRecordService
    {
        // all records loaded from the CSV files.
        public List<RecordItem> AllRecords { get; private set; } = new List<RecordItem>();

        // load data from the three CSV files into AllRecords.
        public void LoadData(string dataFolderPath)
        {
            string recordsPath = Path.Combine(dataFolderPath, "records.csv");
            string namesPath = Path.Combine(dataFolderPath, "names.csv");
            string titlesPath = Path.Combine(dataFolderPath, "titles.csv");

            Frame<int, string> recordsFrame = Frame.ReadCsv(recordsPath, hasHeaders: true);
            Frame<int, string> namesFrame = Frame.ReadCsv(namesPath, hasHeaders: true);
            Frame<int, string> titlesFrame = Frame.ReadCsv(titlesPath, hasHeaders: true);

            var nameLookup = BuildNameLookup(namesFrame);
            var titleLookup = BuildTitleLookup(titlesFrame);

            var list = new List<RecordItem>();

            // eacj row in recordsFrame is one BL record.
            foreach (var row in recordsFrame.Rows.Observations)
            {
                var v = row.Value;

                int blId = GetValueOrDefault(v, "BL record ID", 0);

                string titleFromRecords = GetValueOrDefault(v, "Title", string.Empty);
                string title = GetFromLookup(titleLookup, blId, titleFromRecords);

                string creatorFromRecords = GetValueOrDefault(v, "Name", string.Empty);
                string creator = GetFromLookup(nameLookup, blId, creatorFromRecords);

                string typeOfResource = GetValueOrDefault(v, "Type of resource", string.Empty);
                string genre = GetValueOrDefault(v, "Genre", string.Empty);
                string topics = GetValueOrDefault(v, "Topics", string.Empty);
                string languages = GetValueOrDefault(v, "Languages", string.Empty);
                string dateText = GetValueOrDefault(v, "Date of publication", string.Empty);
                string country = GetValueOrDefault(v, "Country of publication", string.Empty);
                string publisher = GetValueOrDefault(v, "Publisher", string.Empty);

                int? year = ExtractYear(dateText);

                list.Add(new RecordItem
                {
                    BlRecordId = blId,
                    Title = title,
                    CreatorName = creator,
                    TypeOfResource = typeOfResource,
                    Genre = genre,
                    Topics = topics,
                    Languages = languages,
                    Year = year,
                    CountryOfPublication = country,
                    Publisher = publisher,
                    DateOfPublicationRaw = dateText
                });
            }

            AllRecords = list;
        }

        //  genres for the UI dropdown
        public List<string> GetGenres()
        {
            return AllRecords
                .Select(r => r.Genre)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct()
                .OrderBy(g => g)
                .ToList();
        }

        //  resource types for the UI dropdown
        public List<string> GetResourceTypes()
        {
            return AllRecords
                .Select(r => r.TypeOfResource)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }

        // apply all filters and return the matching records
        public List<RecordItem> FilterRecords(
            string titlePart,
            string namePart,
            string selectedGenre,
            string selectedType,
            int? yearFrom,
            int? yearTo)
        {
            IEnumerable<RecordItem> query = AllRecords;

            if (!string.IsNullOrWhiteSpace(titlePart))
            {
                query = query.Where(r =>
                    !string.IsNullOrEmpty(r.Title) &&
                    r.Title.IndexOf(titlePart, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(namePart))
            {
                query = query.Where(r =>
                    !string.IsNullOrEmpty(r.CreatorName) &&
                    r.CreatorName.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(selectedGenre) && selectedGenre != "Any")
            {
                query = query.Where(r =>
                    !string.IsNullOrEmpty(r.Genre) &&
                    string.Equals(r.Genre, selectedGenre, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(selectedType) && selectedType != "Any")
            {
                query = query.Where(r =>
                    !string.IsNullOrEmpty(r.TypeOfResource) &&
                    string.Equals(r.TypeOfResource, selectedType, StringComparison.OrdinalIgnoreCase));
            }

            if (yearFrom.HasValue)
            {
                int from = yearFrom.Value;
                query = query.Where(r => r.Year.HasValue && r.Year.Value >= from);
            }

            if (yearTo.HasValue)
            {
                int to = yearTo.Value;
                query = query.Where(r => r.Year.HasValue && r.Year.Value <= to);
            }

            return query.ToList();
        }

        // buikld the stats text for the current view
        public string BuildStats(IEnumerable<RecordItem> records)
        {
            var currentRecords = records.ToList();

            if (currentRecords.Count == 0)
            {
                return "No records to analyse.";
            }

            int totalRecords = currentRecords.Count;

            int distinctCreators = currentRecords
                .Select(r => r.CreatorName)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .Count();

            var byGenre = currentRecords
                .Where(r => !string.IsNullOrWhiteSpace(r.Genre))
                .GroupBy(r => r.Genre)
                .Select(g => new
                {
                    Genre = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            var lines = new List<string>
            {
                $"Total records in current view: {totalRecords}",
                $"Different creators in current view: {distinctCreators}",
                "",
                "Top genres (by count):"
            };

            foreach (var item in byGenre)
            {
                lines.Add($"- {item.Genre}: {item.Count} records");
            }

            return string.Join(Environment.NewLine, lines);
        }

        //  helpers priv

        private Dictionary<int, string> BuildNameLookup(Frame<int, string> namesFrame)
        {
            var lookup = new Dictionary<int, string>();

            foreach (var row in namesFrame.Rows.Observations)
            {
                var v = row.Value;
                int id = GetValueOrDefault(v, "BL record ID", 0);
                string name = GetValueOrDefault(v, "Name", string.Empty);

                if (id == 0 || string.IsNullOrWhiteSpace(name))
                    continue;

                if (!lookup.ContainsKey(id))
                    lookup[id] = name;
            }

            return lookup;
        }

        private Dictionary<int, string> BuildTitleLookup(Frame<int, string> titlesFrame)
        {
            var lookup = new Dictionary<int, string>();

            foreach (var row in titlesFrame.Rows.Observations)
            {
                var v = row.Value;
                int id = GetValueOrDefault(v, "BL record ID", 0);
                string title = GetValueOrDefault(v, "Title", string.Empty);

                if (id == 0 || string.IsNullOrWhiteSpace(title))
                    continue;

                if (!lookup.ContainsKey(id))
                    lookup[id] = title;
            }

            return lookup;
        }

        private string GetFromLookup(Dictionary<int, string> lookup, int id, string fallback)
        {
            if (lookup != null
                && lookup.TryGetValue(id, out string value)
                && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return fallback;
        }

        private T GetValueOrDefault<T>(ObjectSeries<string> v, string columnName, T defaultValue)
        {
            try
            {
                T value = v.GetAs<T>(columnName);
                if (value == null)
                    return defaultValue;
                return value;
            }
            catch
            {
                return defaultValue;
            }
        }

        private int? ExtractYear(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText))
                return null;

            var match = Regex.Match(dateText, @"\d{4}");
            if (match.Success && int.TryParse(match.Value, out int year))
                return year;

            return null;
        }
    }
}
