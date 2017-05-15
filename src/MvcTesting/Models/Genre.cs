using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.Models
{
    public class Genre
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public IList<FilmGenre> FilmGenres { get; set; }

    }
}
