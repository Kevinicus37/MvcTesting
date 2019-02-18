using MvcTesting.Models;
using System.Collections.Generic;
using TMDbLib.Objects.Movies;

namespace MvcTesting.ViewModels
{
    public class SearchViewModel
    {
        public List<MovieMVC> Movies { get; set; } = new List<MovieMVC>();
        //public int Id { get; set; }
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public string Query { get; set; }
        public string DefaultPosterPath { get; set; } = GlobalVariables.DefaultPoster;

        public SearchViewModel() { }

        public SearchViewModel(List<Movie> movies)
        {
            foreach (Movie movie in movies)
            {
                Movies.Add(new MovieMVC(movie));
            }
        }
    }
}
