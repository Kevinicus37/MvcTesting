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
using MvcTesting.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcTesting.Controllers
{
    public class MovieController : Controller
    {
        private MovieCollectorContext context;
        TMDbClient client;

        public MovieController(MovieCollectorContext dbContext)
        {
            context = dbContext;
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

        
        [HttpGet]
        public IActionResult Add(int id = -1)
        {
            AddMovieViewModel addMovieViewModel;
            List<MediaFormat> mediaFormats = context.MediaFormats.ToList();
            List<AudioFormat> audioFormats = context.AudioFormats.ToList();

            if (id == -1)
            {
                addMovieViewModel = new AddMovieViewModel(mediaFormats, audioFormats);
            }
            else
            {
                Movie movie = client.GetMovieAsync(id, MovieMethods.Credits | MovieMethods.Videos | MovieMethods.Images).Result;
                addMovieViewModel = new AddMovieViewModel(mediaFormats, audioFormats, movie);
            }
            return View(addMovieViewModel);
        }

        [HttpPost]
        [Route("/Movie/Add")]
        public IActionResult Add(AddMovieViewModel addMovieViewModel)
        {
            if (ModelState.IsValid)
            {
                
                return View("Test", addMovieViewModel);
            }
            List<SelectListItem> mediaFormats = new List<SelectListItem> { new SelectListItem {
                Value= 0.ToString(), Text ="DVD" }, new SelectListItem {Value = 1.ToString(), Text ="Bluray" } };

            List<SelectListItem> audioFormats = new List<SelectListItem> { new SelectListItem { Value = 0.ToString(), Text = "Atmos" },
                new SelectListItem {Value=1.ToString(), Text= "DTS:X" } };

            addMovieViewModel.MediaFormats = mediaFormats;
            addMovieViewModel.AudioFormats = audioFormats;
            
            return View(addMovieViewModel);
        }
    }

    
}
