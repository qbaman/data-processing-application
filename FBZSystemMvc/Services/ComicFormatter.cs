using System;
using System.Collections.Generic;
using System.Linq;
using FBZ_System.Domain;

namespace FBZ_System.Services
{
    public class ComicFormatter // ONLY FORMATS COMIC OBJECTS INTO DISPLAYABLE STRINGS. SINGLE RESPONSIBILITY 
    {
        public string FormatTitle(Comic comic)
        {
            if (comic == null)
                return string.Empty;

            var title = comic.MainTitle;
            if (string.IsNullOrWhiteSpace(title))
                return "Unknown title";

            return Clean(title);
        }

        public string FormatAuthors(Comic comic)
        {
            return FormatMulti("Author", comic?.Authors);
        }

        public string FormatYears(Comic comic)
        {
            if (comic?.Years == null || comic.Years.Count == 0)
                return "Unknown";

            var ordered = comic.Years
                .Distinct()
                .OrderBy(y => y);

            return string.Join(", ", ordered);
        }

        public string FormatGenres(Comic comic)
        {
            return FormatMulti("Genre", comic?.Genres);
        }

        public List<string> BuildInfoLines(Comic comic)
        {
            var lines = new List<string>();
            if (comic == null) return lines;

            AddMultiLine(lines, "Variant titles", comic.VariantTitles);
            AddMultiLine(lines, "Languages", comic.Languages);
            AddMultiLine(lines, "Editions", comic.Editions);
            AddMultiLine(lines, "Name types", comic.NameTypes);

            if (comic.ExtraAttributes != null && comic.ExtraAttributes.Count > 0)
            {
                foreach (var entry in comic.ExtraAttributes.OrderBy(e => e.Key))
                {
                    var label = Clean(entry.Key);

                    foreach (var raw in entry.Value ?? new List<string>())
                    {
                        if (string.IsNullOrWhiteSpace(raw)) continue;

                        lines.Add($"{label}: {NormalizeKeyValue(raw)}");
                    }
                }
            }

            return lines;
        }

        private void AddMultiLine(List<string> lines, string label, IEnumerable<string>? values)
        {
            if (values == null) return;

            var list = values
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(Clean)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (list.Count == 0) return;

            // one line per value (kiosk-friendly)
            foreach (var v in list)
                lines.Add($"{label}: {v}");
        }

        private string NormalizeKeyValue(string raw)
        {
            raw = Clean(raw);

            var idx = raw.IndexOf(':');
            if (idx < 0) return raw;

            var k = raw.Substring(0, idx).Trim();
            var v = raw.Substring(idx + 1).Trim();

            if (k.Length == 0) return raw;
            if (v.Length == 0) return $"{k}:";

            return $"{k}: {v}";
        }


        public string FormatIsbns(Comic comic)
        {
            if (comic?.Isbns == null || comic.Isbns.Count == 0)
                return "ISBN: [missing]";

            return FormatMulti("ISBN", comic.Isbns);
        }


        private string FormatMulti(string label, IEnumerable<string>? values)
        {
            if (values == null)
                return $"{label}: [none]";

            var list = values
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(Clean)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (list.Count == 0)
                return $"{label}: [none]";

            return $"{label}: " + string.Join("; ", list.Select(v => $"[{v}]"));
        }

        private string Clean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var s = value.Replace("�", "'").Trim();
            return s;
        }
    }
}
