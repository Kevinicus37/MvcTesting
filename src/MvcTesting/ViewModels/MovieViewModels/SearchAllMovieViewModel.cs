using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class SearchAllMovieViewModel : SearchAndDisplayViewModel
    {
        public SearchAllMovieViewModel() { }

        public SearchAllMovieViewModel(IList<Film> films) : base(films)
        {

        }
    }
}
