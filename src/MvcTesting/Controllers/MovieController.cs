using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using MvcTesting.ViewModels;
using TMDbLib.Objects.Movies;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcTesting.Controllers
{
    public class MovieController : Controller
    {

        TMDbClient client;

        public MovieController()
        {
            client = new TMDbClient("9950b6bfd3eef8b5c9b7343ead080098");
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(string query)
        {
            List<Movie> movies = new List<Movie>();
            SearchContainer<SearchMovie> results = client.SearchMovieAsync(query, 1).Result;
            foreach (var result in results.Results)
            {
                Movie aMovie = client.GetMovieAsync(result.Id, MovieMethods.Credits | MovieMethods.Videos | MovieMethods.Images).Result;
                movies.Add(aMovie);
            }
            SearchViewModel searchViewModel = new SearchViewModel(movies);
            return View(searchViewModel);
        }

        public IActionResult ViewMovie(int Id)
        {

            Movie movie = client.GetMovieAsync(Id, MovieMethods.Credits | MovieMethods.Videos | MovieMethods.Images).Result;
            return View(movie);
        }
    }

    
}
