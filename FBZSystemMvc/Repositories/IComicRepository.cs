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

        IEnumerable<string> GetAllPhysicalDescriptions();

        IEnumerable<string> GetAllResourceTypes();

        IEnumerable<string> GetAllLanguages();

        IEnumerable<string> GetAllEditions();

        IEnumerable<string> GetAllTopics(); 
        
        IEnumerable<string> GetAllContentTypes();

        IReadOnlyList<Comic> GetByGenres(IEnumerable<string> genres);
    }
}
