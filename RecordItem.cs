using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBZ_System
{
    // This class represents one library record, combined
    // from the names, titles and records CSV files.
    public class RecordItem
    {
        // Unique BL record ID.
        public int BlRecordId { get; set; }

        // Main title (from titles/records tables).
        public string Title { get; set; }

        // Main creator (author / artist).
        public string CreatorName { get; set; }

        // Type of resource, e.g. "Monograph" or "Serial".
        public string TypeOfResource { get; set; }

        // Genre, e.g. "Comic or graphic novel".
        public string Genre { get; set; }

        // Topics / subjects.
        public string Topics { get; set; }

        // Languages.
        public string Languages { get; set; }

        // Approximate publication year (first 4-digit year found).
        public int? Year { get; set; }

        // Country where it was published.
        public string CountryOfPublication { get; set; }

        // Publisher name.
        public string Publisher { get; set; }

        // Full date text from CSV (for reference).
        public string DateOfPublicationRaw { get; set; }


    }
}
