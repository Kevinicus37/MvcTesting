using System.Collections.Generic;
using TMDbLib.Objects.Movies;

namespace MvcTesting.ViewModels
{
    public class SearchViewModel
    {
        public List<Movie> Movies { get; set; }
        public int Id { get; set; }
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public string Query { get; set; }

        public SearchViewModel() { }

        public SearchViewModel(List<Movie> movies)
        {
            Movies = movies;
        }
    }
}
