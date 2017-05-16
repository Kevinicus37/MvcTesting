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
            List<Film> films = context.Films.OrderBy(f => f.Name).OrderBy(f => f.Year).ToList();
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
            return View("Results", searchViewModel);
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
                Film film = new Film();
                int id = UpdateMovie(addMovieViewModel, film);
                return Redirect($"/Movie/ViewMovie/{id}");
            }

            addMovieViewModel.MediaFormats = addMovieViewModel.PopulateList(context.MediaFormats.ToList());
            addMovieViewModel.AudioFormats = addMovieViewModel.PopulateList(context.AudioFormats.ToList());

            return View(addMovieViewModel);
        }

        public IActionResult ViewMovie(int id)
        {
            Film film = context.Films.Include(f => f.Media).Include(f => f.Audio).Single(f => f.ID == id);
            List<FilmGenre> genres = context.FilmGenres.Include(g => g.Genre).Where(f => f.FilmID == id).ToList();
            ViewMovieViewModel viewMovieViewModel = new ViewMovieViewModel(film, genres);
            return View(viewMovieViewModel);

        }

        public IActionResult Remove()
        {
            List<Film> films = context.Films.OrderBy(f => f.Name).OrderBy(f => f.Year).ToList();
            return View(films);
        }

        [HttpPost]
        public IActionResult Remove(int[] filmIds)
        {
            foreach (int id in filmIds)
            {
                Film oldFilm = context.Films.Single(f => f.ID == id);
                context.Films.Remove(oldFilm);

            }

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Film editMovie = context.Films.Single(f => f.ID == id);
            List<MediaFormat> mediaFormats = context.MediaFormats.ToList();
            List<AudioFormat> audioFormats = context.AudioFormats.ToList();
            EditMovieViewModel editMovieViewModel = new EditMovieViewModel(mediaFormats, audioFormats, editMovie);
            editMovieViewModel.ID = id;
            return View(editMovieViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditMovieViewModel editMovieViewModel)
        {
            if (ModelState.IsValid)
            {
                Film film = context.Films.Single(f => f.ID == editMovieViewModel.ID);
                int id = UpdateMovie(editMovieViewModel, film);
                return Redirect($"/Movie/ViewMovie/{id}");
            }

            editMovieViewModel.MediaFormats = editMovieViewModel.PopulateList(context.MediaFormats.ToList());
            editMovieViewModel.AudioFormats = editMovieViewModel.PopulateList(context.AudioFormats.ToList());

            return View(editMovieViewModel);
        }

        public int UpdateMovie(AddMovieViewModel viewModel, Film film)
        {   
            // UpdateMovie transfers data from viewModel to the Film object.  The created
            // or edited film's ID is returned as an int.

            MediaFormat newMediaFormat = context.MediaFormats.Single(m => m.ID == viewModel.MediaID);
            AudioFormat newAudioFormat = context.AudioFormats.Single(a => a.ID == viewModel.AudioID);
            film.Name = viewModel.Name;
            film.Year = (int)viewModel.Year;
            film.AspectRatio = viewModel.AspectRatio;
            film.TMDbId = viewModel.TMDbId;
            film.Comments = viewModel.Comments;
            film.Rating = viewModel.Rating;
            film.Directors = viewModel.Directors;
            film.Cast = viewModel.Cast;
            film.Overview = viewModel.Overview;
            film.TrailerUrl = viewModel.TrailerUrl;
            film.PosterUrl = viewModel.PosterUrl;
            film.IsPrivate = viewModel.IsPrivate;
            film.Has3D = viewModel.Has3D;
            film.Audio = newAudioFormat;
            film.Media = newMediaFormat;
            
            if (viewModel.Genres != null)
            {
                List<FilmGenre> thisFilmGenres = context.FilmGenres.Include(fg => fg.Genre).Where(fg => fg.FilmID == film.ID).ToList();

                // Eliminate any previous FilmGenres that weren't selected in an edit
                foreach (FilmGenre filmGenre in thisFilmGenres)
                {
                    if (!viewModel.Genres.Contains(filmGenre.Genre.Name))
                    {
                        context.FilmGenres.Remove(filmGenre);
                    }
                }

                // Add selected Genres to a film <FilmGenre> if it doesn't already exist
                foreach (string genre in viewModel.Genres)
                {
                    // Gets a list of existing FilmGenre items to check against.
                    IList<FilmGenre> existingFilmGenres = context.FilmGenres.Where(fg => fg.FilmID == film.ID)
                        .Where(fg => fg.Genre.Name == genre).ToList();

                    Models.Genre newGenre = context.Genres.Single(g => g.Name == genre);

                    // If no FilmGenres with the current FilmID and GenreID exist, one is created.
                    if (existingFilmGenres.Count == 0)
                    {
                        FilmGenre newFilmGenre = new FilmGenre
                        {
                            FilmID = film.ID,
                            GenreID = newGenre.ID
                        };

                        context.FilmGenres.Add(newFilmGenre);
                    }
                    
                }
            }

            context.SaveChanges();
            return film.ID;
        }
    }  

    
}
