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
using Microsoft.EntityFrameworkCore;

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
            List<Film> films = context.Films.OrderBy(f => f.Name).ToList();
            return View(films);
        }

        public IActionResult Search()
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
            return View("Results",searchViewModel);
        }

        public IActionResult ViewSearchedMovie(int Id)
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
            // If the model is valid, create a new film and add it to the database.
            if (ModelState.IsValid)
            {

                MediaFormat newMediaFormat = context.MediaFormats.Single(m => m.ID == addMovieViewModel.MediaID);
                AudioFormat newAudioFormat = context.AudioFormats.Single(a => a.ID == addMovieViewModel.AudioID);
                Film newFilm = new Film
                {
                    Name = addMovieViewModel.Name,
                    Year = (int)addMovieViewModel.Year,
                    AspectRatio = addMovieViewModel.AspectRatio,
                    TMDbId = addMovieViewModel.TMDbId,
                    Comments = addMovieViewModel.Comments,
                    Rating = addMovieViewModel.Rating,
                    Directors = addMovieViewModel.Directors,
                    Cast = addMovieViewModel.Cast,
                    Overview = addMovieViewModel.Overview,
                    TrailerUrl = addMovieViewModel.TrailerUrl,
                    PosterUrl = addMovieViewModel.PosterUrl,
                    IsPrivate = addMovieViewModel.IsPrivate,
                    Has3D = addMovieViewModel.Has3D,
                    Audio = newAudioFormat,
                    Media= newMediaFormat

                };

                context.Films.Add(newFilm);
                // context.SaveChanges();

                if (addMovieViewModel.Genres != null)
                {
                    foreach (string genre in addMovieViewModel.Genres)
                    {

                        IList<FilmGenre> existingFilmGenres = context.FilmGenres.Where(fg => fg.FilmID == newFilm.ID)
                            .Where(fg => fg.Genre.Name == genre).ToList();

                        MvcTesting.Models.Genre newGenre = context.Genres.Single(g => g.Name == genre);

                        if (existingFilmGenres.Count == 0)
                        {
                            FilmGenre newFilmGenre = new FilmGenre
                            {
                                FilmID = newFilm.ID,
                                GenreID = newGenre.ID
                            };

                            context.FilmGenres.Add(newFilmGenre);
                            //context.SaveChanges();
                        }
                    }
                }
                context.SaveChanges();
                return Redirect($"/Movie/ViewMovie/{newFilm.ID}");
            }
            
            addMovieViewModel.MediaFormats = addMovieViewModel.PopulateList(context.MediaFormats.ToList());
            addMovieViewModel.AudioFormats = addMovieViewModel.PopulateList(context.AudioFormats.ToList());
            
            return View(addMovieViewModel);
        }

        public IActionResult ViewMovie(int id)
        {
            Film film = context.Films.Include(f => f.Media).Include(f=> f.Audio).Single(f=>f.ID==id);
            List<FilmGenre> genres = context.FilmGenres.Include(g => g.Genre).Where(f => f.FilmID == id).ToList();
            ViewMovieViewModel viewMovieViewModel = new ViewMovieViewModel(film, genres);
            return View(viewMovieViewModel);
        }
    }

    
}
