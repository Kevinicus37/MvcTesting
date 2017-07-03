using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
