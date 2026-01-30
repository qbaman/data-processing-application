using FBZ_System.Domain;

namespace FBZ_System.Services
{
    public interface IComicFormatter
    {
        string FormatTitle(Comic comic);
        string FormatAuthors(Comic comic);
        string FormatYears(Comic comic);
        string FormatGenres(Comic comic);
        string FormatIsbns(Comic comic);
    }
}
