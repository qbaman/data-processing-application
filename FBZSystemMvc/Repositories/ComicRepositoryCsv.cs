using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deedle;
using FBZ_System.Domain;

namespace FBZ_System.Repositories
{
    public class ComicRepositoryCsv : IComicRepository
    {
        private readonly List<Comic> _comics = new();

        public ComicRepositoryCsv(string csvFolderPath)
        {
            var filePath = Path.Combine(csvFolderPath, "names.csv");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("CSV file not found", filePath);

            var frame = Frame.ReadCsv(filePath);

            var comicsById = new Dictionary<string, Comic>(StringComparer.OrdinalIgnoreCase);

            foreach (var obs in frame.Rows.Observations)
            {
                var row = obs.Value;

                string id = GetString(row, "BL record ID");
                string title = GetString(row, "Title");
                string variantTitle = GetString(row, "Variant titles");
                string name = GetString(row, "Name");
                string typeOfName = GetString(row, "Type of name");
                string isbn = GetString(row, "ISBN");
                string genre = GetString(row, "Genre");
                string languages = GetString(row, "Languages");
                string yearText = GetString(row, "Date of publication");
                string edition = GetString(row, "Edition");

                // Extra metadata fields (for key:value display)
                string typeOfResource = GetString(row, "Type of resource");
                string contentType = GetString(row, "Content type");
                string materialType = GetString(row, "Material type");
                string physicalDescription = GetString(row, "Physical description");
                string dewey = GetString(row, "Dewey classification");
                string topics = GetString(row, "Topics");

                // skip empty rows
                if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(title))
                    continue;

                if (!comicsById.TryGetValue(id, out var comic))
                {
                    comic = new Comic { Id = id };
                    comicsById[id] = comic;
                }

                // main title + variant titles
                if (!string.IsNullOrWhiteSpace(title))
                {
                    if (string.IsNullOrWhiteSpace(comic.MainTitle))
                        comic.MainTitle = title;
                    else
                        AddUnique(comic.VariantTitles, title);
                }

                AddMultiValue(comic.VariantTitles, variantTitle);

                // Authors
                AddMultiValue(comic.Authors, name);

                // Name type
                AddMultiValue(comic.NameTypes, typeOfName);

                // Genres
                AddMultiValue(comic.Genres, genre);

                // Languages
                AddMultiValue(comic.Languages, languages);

                // Editions
                AddMultiValue(comic.Editions, edition);

                // Year
                if (int.TryParse(yearText, out int year))
                    AddUnique(comic.Years, year);

                // ISBN
                AddMultiValue(comic.Isbns, isbn);

                // Extra attributes (semicolon-separated, often contains "Key: Value" parts)
                AddExtra(comic, "Type of resource", typeOfResource);
                AddExtra(comic, "Content type", contentType);
                AddExtra(comic, "Material type", materialType);
                AddExtra(comic, "Physical description", physicalDescription);
                AddExtra(comic, "Dewey classification", dewey);
                AddExtra(comic, "Topics", topics);
            }

            foreach (var c in comicsById.Values)
            {
                // if no isbn mark "missing"
                if (c.Isbns.Count == 0)
                    c.Isbns.Add("missing");

                _comics.Add(c);
            }
        }

        // Return all comics
        public IReadOnlyList<Comic> GetAllComics() => _comics;

        // Return comics that have one of the specified genres
        public IReadOnlyList<Comic> GetByGenres(IEnumerable<string> genres)
        {
            var genreSet = new HashSet<string>(genres ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            if (genreSet.Count == 0)
                return _comics.ToList();

            return _comics
                .Where(c => c.Genres != null && c.Genres.Any(g => genreSet.Contains(g)))
                .ToList();
        }

        // REQUIRED by your interface (this fixes your build error)
        public IEnumerable<string> GetAllGenres()
        {
            return _comics
                .SelectMany(c => c.Genres ?? new List<string>())
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Select(g => g.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g)
                .ToList();
        }

        public IEnumerable<string> GetAllNameTypes()
        {
            return GetAllComics()
                .SelectMany(c => c.NameTypes ?? new List<string>())
                .Select(x => (x ?? "").Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }

        public IEnumerable<string> GetAllResourceTypes()
        {
            return GetAllComics()
                .SelectMany(c =>
                    (c.ExtraAttributes != null &&
                    c.ExtraAttributes.TryGetValue("Type of resource", out var vals) &&
                    vals != null)
                        ? vals
                        : new List<string>())
                .Select(x => (x ?? "").Trim())
                .Where(x => x.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }

        public IEnumerable<string> GetAllTopics()
        {
            return GetAllComics()
                .SelectMany(c =>
                    (c.ExtraAttributes != null &&
                    c.ExtraAttributes.TryGetValue("Topics", out var vals) &&
                    vals != null)
                        ? vals
                        : new List<string>())
                .Select(x => (x ?? "").Trim())
                .Where(x => x.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }

        // helpers

        private static string GetString(Series<string, object> row, string column)
        {
            if (row == null || string.IsNullOrWhiteSpace(column))
                return string.Empty;

            var opt = row.TryGet(column);
            if (!opt.HasValue)
                return string.Empty;

            var obj = opt.Value;
            if (obj == null)
                return string.Empty;

            var s = obj.ToString() ?? string.Empty;
            return s.Trim();
        }

        private static void AddUnique(List<string> target, string value)
        {
            if (target == null || string.IsNullOrWhiteSpace(value))
                return;

            value = value.Trim();
            if (value.Length == 0)
                return;

            if (!target.Exists(x => string.Equals(x, value, StringComparison.OrdinalIgnoreCase)))
                target.Add(value);
        }

        public IEnumerable<string> GetAllLanguages()
        {
            return GetAllComics()
                .SelectMany(c => c.Languages ?? new List<string>())
                .Select(x => (x ?? "").Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }

        public IEnumerable<string> GetAllEditions()
        {
            return GetAllComics()
                .SelectMany(c => c.Editions ?? new List<string>())
                .Select(x => (x ?? "").Trim())
                .Where(x => x.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }

        private static void AddUnique(List<int> target, int value)
        {
            if (target == null)
                return;

            if (!target.Contains(value))
                target.Add(value);
        }

        // NOTE: split ONLY by semicolon to avoid breaking values containing commas (e.g., names)
        private static void AddMultiValue(List<string> target, string raw)
        {
            if (target == null || string.IsNullOrWhiteSpace(raw))
                return;

            foreach (var part in raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = part.Trim();
                if (trimmed.Length == 0)
                    continue;

                if (!target.Exists(x => string.Equals(x, trimmed, StringComparison.OrdinalIgnoreCase)))
                    target.Add(trimmed);
            }
        }

        // ExtraAttributes: split ONLY by semicolon (preserve commas), keep "Key: Value" parts as-is
        private static void AddExtra(Comic comic, string key, string raw)
        {
            if (comic == null || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(raw))
                return;

            if (!comic.ExtraAttributes.TryGetValue(key, out var list))
            {
                list = new List<string>();
                comic.ExtraAttributes[key] = list;
            }

            foreach (var part in raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = part.Trim();
                if (trimmed.Length == 0)
                    continue;

                if (!list.Exists(x => string.Equals(x, trimmed, StringComparison.OrdinalIgnoreCase)))
                    list.Add(trimmed);
            }
        }
    }
}
