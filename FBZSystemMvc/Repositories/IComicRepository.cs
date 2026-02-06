using System.Collections.Generic;
using FBZ_System.Domain;

namespace FBZ_System.Repositories
{
    public interface IComicRepository // ONLY DEALS WITH COMIC DATA METHODS. ISP
    {
        //  read-only lists of Comic
        IReadOnlyList<Comic> GetAllComics();

        IEnumerable<string> GetAllGenres();

        IEnumerable<string> GetAllNameTypes();

        IEnumerable<string> GetAllLanguages();


        IReadOnlyList<Comic> GetByGenres(IEnumerable<string> genres);
    }
}
