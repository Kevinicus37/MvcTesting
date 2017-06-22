using System.Collections.Generic;

namespace MvcTesting.Models
{
    public class Genre : MediaProperty
    {
        public IList<FilmGenre> FilmGenres { get; set; }

    }
}
