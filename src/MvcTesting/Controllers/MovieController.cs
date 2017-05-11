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

        
        [HttpGet]
        public IActionResult Add(int id = -1)
        {
            AddMovieViewModel addMovieViewModel;
            List<Models.MediaType> mediaTypes = new List<Models.MediaType> { new Models.MediaType {
                ID= 1, Name ="DVD" }, new Models.MediaType {ID = 2, Name ="Bluray" } };

            List<AudioFormat> audioFormats = new List<AudioFormat> { new AudioFormat { ID = 1, Name = "Atmos" },
                new AudioFormat {ID=2, Name= "DTS:X" } };

            if (id == -1)
            {
                addMovieViewModel = new AddMovieViewModel(mediaTypes, audioFormats);
            }
            else
            {
                Movie movie = client.GetMovieAsync(id, MovieMethods.Credits | MovieMethods.Videos | MovieMethods.Images).Result;
                addMovieViewModel = new AddMovieViewModel(mediaTypes, audioFormats, movie);
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
            List<SelectListItem> mediaTypes = new List<SelectListItem> { new SelectListItem {
                Value= 0.ToString(), Text ="DVD" }, new SelectListItem {Value = 1.ToString(), Text ="Bluray" } };

            List<SelectListItem> audioFormats = new List<SelectListItem> { new SelectListItem { Value = 0.ToString(), Text = "Atmos" },
                new SelectListItem {Value=1.ToString(), Text= "DTS:X" } };

            addMovieViewModel.MediaTypes = mediaTypes;
            addMovieViewModel.AudioFormats = audioFormats;
            
            return View(addMovieViewModel);
        }
    }

    
}
