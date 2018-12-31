using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class SearchMovieViewModel : SearchAndDisplayViewModel
    {
        public SearchMovieViewModel() { }

        public SearchMovieViewModel(IList<Film> films) : base(films)
        {

        }
    }
}
