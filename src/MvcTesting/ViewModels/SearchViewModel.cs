using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;

namespace MvcTesting.ViewModels
{
    public class SearchViewModel
    {
        public List<Movie> Movies { get; set; }
        public int Id { get; set; }

        public SearchViewModel() { }

        public SearchViewModel(List<Movie> movies)
        {
            Movies = movies;
        }
    }
}
