using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deedle;
using FBZ_System.Domain;

namespace FBZ_System.Repositories
{
    public class ComicRepositoryCsv : IComicRepository // THIS ONLY LOADS CSV FROM COMIC, SINGLE RESPONSIBILITY.
    {
        private readonly List<Comic> _comics = new();

        public IEnumerable<string> GetAllGenres()
        {
            return _comics
                .SelectMany(c => c.Genres ?? Enumerable.Empty<string>())
                .Select(g => g.Trim())
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g);
        }


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

                // skip empty rows
                if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(title))
                    continue;

                if (!comicsById.TryGetValue(id, out var comic))
                {
                    comic = new Comic
                    {
                        Id = id
                    };
                    comicsById[id] = comic;
                }

                // main title + variant titles
                if (!string.IsNullOrWhiteSpace(title))
                {
                    if (string.IsNullOrWhiteSpace(comic.MainTitle))
                    {
                        comic.MainTitle = title;
                    }
                    else
                    {
                        AddUnique(comic.VariantTitles, title);
                    }
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
                {
                    AddUnique(comic.Years, year);
                }

                // ISBN
                AddMultiValue(comic.Isbns, isbn);
            }

            foreach (var c in comicsById.Values)
            {
                // if no isbn mark "missing"
                if (c.Isbns.Count == 0)
                {
                    c.Isbns.Add("missing");
                }

                _comics.Add(c);
            }
        }

        // Return all comics
        public IReadOnlyList<Comic> GetAllComics() => _comics;

        // Return comics that have one of the specified genres
        public IReadOnlyList<Comic> GetByGenres(IEnumerable<string> genres)
        {
            var genreSet = new HashSet<string>(genres ?? Array.Empty<string>(),
                                               StringComparer.OrdinalIgnoreCase);

            if (genreSet.Count == 0)
                return _comics.ToList();

            return _comics
                .Where(c => c.Genres != null &&
                            c.Genres.Any(g => genreSet.Contains(g)))
                .ToList();
        }

        //  helpers 

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
            {
                target.Add(value);
            }
        }

        private static void AddUnique(List<int> target, int value)
        {
            if (target == null)
                return;

            if (!target.Contains(value))
                target.Add(value);
        }

        private static void AddMultiValue(List<string> target, string raw)
        {
            if (target == null || string.IsNullOrWhiteSpace(raw))
                return;

            foreach (var part in raw.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = part.Trim();
                if (trimmed.Length == 0)
                    continue;

                if (!target.Exists(x => string.Equals(x, trimmed, StringComparison.OrdinalIgnoreCase)))
                    target.Add(trimmed);
            }
        }
    }
}
