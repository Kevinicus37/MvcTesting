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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
//using MvcTesting.Data;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcTesting.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private MovieCollectorContext _context;
        
        TMDbClient client;

        public MovieController(MovieCollectorContext dbContext,  UserManager<ApplicationUser> userManager)
        {
            // Initialize items to be used throughout the controller

            _userManager = userManager;
            _context = dbContext;

            // This is the client that is used to work with the TMDb.org API
            client = new TMDbClient("9950b6bfd3eef8b5c9b7343ead080098");
        }

        // GET: /<controller>/
        [AllowAnonymous]
        public IActionResult Index()
        {

            bool userCanSeePrivate = false;

            if (User.IsInRole("Admin"))
            {
                userCanSeePrivate = true;
            }
            //Gets a list of all films in order of Name, and then by Year and displays it in the view.
            List<Film> films = _context.Films.Include(f=>f.User).OrderBy(f => f.Name)
                .Where(f=> (!f.IsPrivate && !f.User.IsPrivate) || f.UserID == _userManager.GetUserId(User) || userCanSeePrivate)
                .OrderBy(f => f.Year)
                .ToList();
            MovieIndexViewModel movieIndexViewModel = new MovieIndexViewModel(films);
            return View(movieIndexViewModel);
        }

        public IActionResult Search()
        {
            // Displays view allowing a user to search for a movie (using TMDb) to add to their collection.
            return View();
        }

        [HttpPost]
        public IActionResult Search(string query, int page)
        {
            // Accepts a search query and passes it to TMDb.org's API and accepts the search results.
            int currentPage = 1;
            int lastPage = 1;
            
            List<Movie> movies = new List<Movie>();

            // This is the request to the TMDb API.  It returns a container of movies.
            // Null is returned if the site cannot be reached and there is an exception.
            SearchContainer<SearchMovie> results = TMDbSearch(client, query, page);
            
            // Credits, Video, and Image information is pulled for each movie in the search results.
            if (results != null && results.Results != null)
            {
                lastPage = results.TotalPages;
                currentPage = page;
                
                foreach (var result in results.Results)
                {
                    Movie aMovie = GetTMDbMovieInfo(result.Id);
                    if (aMovie != null)
                    {
                        movies.Add(aMovie);
                    }
                }
            }
            SearchViewModel searchViewModel = new SearchViewModel(movies);
            searchViewModel.CurrentPage = currentPage;
            searchViewModel.LastPage = lastPage;
            searchViewModel.Query = query;
            return View("Results", searchViewModel);
        }

        [AllowAnonymous]
        public IActionResult SearchAll(string query)
        {
            
            List<Film> films = _context.Films
                .Include(f=>f.User)
                .Where(f => (!f.IsPrivate && !f.User.IsPrivate) || User.IsInRole("Admin") || f.UserID == _userManager.GetUserId(User))
                .Where(f => f.Name.ToLower().Contains(query.ToLower()))
                .ToList();

            var vm = new SearchAllMovieViewModel(films);

            vm.Genres = _context.Genres.ToList();
            vm.MediaFormats = _context.MediaFormats.ToList();
            vm.AudioFormats = _context.AudioFormats.ToList();
            
            
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult SearchAll(SearchAllMovieViewModel vm)
        {
            List<Film> films = new List<Film>();
            
            // If someone attempts to access a page for a private User directly and is not that User
            // and is not an Admin, they are redirected to the Index Action.
            if (!string.IsNullOrEmpty(vm.SearchValue))
            {
                switch (vm.PropertyType)
                {
                    case "Genre":
                        vm.FilterValue = (!_context.Genres.Any(g => g.Name == vm.FilterValue) ? null : vm.FilterValue);
                        films = GetAllFilmsByGenre(vm.SearchValue, vm.FilterValue);
                        break;
                    case "MediaFormat":
                        vm.FilterValue = (!_context.MediaFormats.Any(mf => mf.Name == vm.FilterValue) ? null : vm.FilterValue);
                        films = GetAllFilmsByMediaFormat(vm.SearchValue, vm.FilterValue);
                        break;
                    case "AudioFormat":
                        vm.FilterValue = (!_context.AudioFormats.Any(af => af.Name == vm.FilterValue) ? null : vm.FilterValue);
                        films = GetAllFilmsByAudioFormat(vm.SearchValue, vm.FilterValue);
                        break;
                    case "Film":
                        string title = (!_context.Films.Any(f => f.Name.ToLower().Contains(vm.SearchValue.ToLower())) ? null : vm.SearchValue);

                        if (!string.IsNullOrEmpty(title))
                        {
                            films = GetFilmsByTitle(title).ToList();
                        }

                        break;
                    default:
                        films = GetFilmsByTitle(vm.SearchValue).ToList();
                        break;

                }

                vm.Films = FilmSortingHelpers.SortByValue(films, vm.SortValue);
            }
            else
            {
                vm.Films = films;
            }

            
            vm.Genres = _context.Genres.ToList();
            vm.MediaFormats = _context.MediaFormats.ToList();
            vm.AudioFormats = _context.AudioFormats.ToList();

            return View(vm);
        }

        public IActionResult ViewSearchedMovie(int Id)
        {
            // This displays a closer look at an individual movie when it is selected.
            Movie movie = GetTMDbMovieInfo(Id);
            if (movie != null)
            {
                return View(movie);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add(int? id)
        {
            // Get Audio Formats, and Media Formats arguments to pass into ViewModel
            // to generate selection options.
            List<MediaFormat> mediaFormats = _context.MediaFormats.ToList();
            List<AudioFormat> audioFormats = _context.AudioFormats.ToList();
            
            Movie movie = null;

            // If id has a value, then try to get a movie object from TMDb (Null if not found).
            if (id.HasValue)
            {
                movie = GetTMDbMovieInfo((int)id);
            }

            AddMovieViewModel addMovieViewModel = new AddMovieViewModel(mediaFormats, audioFormats, movie);
            addMovieViewModel.AvailableGenres = _context.Genres.ToList();

            return View(addMovieViewModel);
        }

        [HttpPost]
        [Route("/Movie/Add")]
        public async Task<IActionResult> Add(AddMovieViewModel addMovieViewModel)
        {
            // If the model is valid, create a new film and add it to the database. If
            // it's not valid, return to the view.
            if (ModelState.IsValid)
            {
                ApplicationUser user = _context.Users.Single(u => u.Id == _userManager.GetUserId(User));
                Film film = new Film();
                film.User = user;
                int id = await UpdateMovieAsync(addMovieViewModel, film);
                return Redirect($"/Movie/ViewMovie/{id}");
            }

            addMovieViewModel.MediaFormats = addMovieViewModel.PopulateList(_context.MediaFormats.ToList());
            addMovieViewModel.AudioFormats = addMovieViewModel.PopulateList(_context.AudioFormats.ToList());

            return View(addMovieViewModel);
        }

        [AllowAnonymous]
        public IActionResult ViewMovie(int id)
        {

            // Finds and displays a Film based on the ID.  If no Film is found with a matching ID,
            // the User is returned to the Index action.
            Film film = _context.Films.Include(f => f.Media).Include(f => f.Audio).SingleOrDefault(f => f.ID == id);
            if (film != null)
            {
                List<FilmGenre> genres = _context.FilmGenres.Include(g => g.Genre).Where(f => f.FilmID == id).ToList();
                ViewMovieViewModel viewMovieViewModel = new ViewMovieViewModel(film, genres);
                return View(viewMovieViewModel);
            }
            return RedirectToAction("Index");
            

        }

        public IActionResult Remove()
        {
            // Display a list of films in the current User's collection that can be removed.
            // I might want to create a ViewModel instead of passing the List directly to the View.
            List<Film> films = _context.Films.OrderBy(f=>f.Name).OrderBy(f=>f.Year).Where(f => f.UserID== _userManager.GetUserId(User)).ToList();
            return View(films);
        }

        [HttpPost]
        public IActionResult Remove(int[] filmIds)
        {

            // Goes through the list of returned filmIds and if the Film belongs to the active user,
            // or the active user is an Admin, the Film is deleted.  The user is then returned to the
            // Index Action.
            foreach (int id in filmIds)
            {

                //Film oldFilm = _context.Films.SingleOrDefault(f => f.ID == id);
                Film oldFilm = _context.Films
                    .Include(f => f.User)
                    .SingleOrDefault(f => f.ID == 9010);
                
                if (oldFilm.User.UserName == _userManager.GetUserName(User) || User.IsInRole("Admin"))
                {
                    _context.Films.Remove(oldFilm);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            // Allows user to edit Properties of one of the Films in their Collection

            Film editFilm = _context.Films.SingleOrDefault(f => f.ID == id);

            // If editFilm with the passed in id exists and the User is associated with the Film or Admin,
            // then the ViewModel is seeded.  Otherwise the User is redirected to the Index Action.
            if (editFilm != null)
            {
                if (editFilm.UserID == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    List<MediaFormat> mediaFormats = _context.MediaFormats.ToList();
                    List<AudioFormat> audioFormats = _context.AudioFormats.ToList();
                    EditMovieViewModel editMovieViewModel = new EditMovieViewModel(mediaFormats, audioFormats, editFilm);
                    editMovieViewModel.Genres = GetGenres(id);
                    editMovieViewModel.ID = id;
                    editMovieViewModel.AvailableGenres = _context.Genres.ToList();
                    return View(editMovieViewModel);
                }
            }
            
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EditMovieViewModel editMovieViewModel)
        {
            // Verify model data is valid and that the active User is the owner of the Film
            // or Admin.  If so, the Film is then updated.  The User is Redirected to the page of
            // the Film.  

            if (ModelState.IsValid)
            {

                Film film = _context.Films.Single(f => f.ID == editMovieViewModel.ID);
                if (film.UserID == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    int id = await UpdateMovieAsync(editMovieViewModel, film);
                    return Redirect($"/Movie/ViewMovie/{id}");
                }
                
                return RedirectToAction("Index");
            }

            // If the model is not valid, it is re-seeded and returned to the View.
            editMovieViewModel.MediaFormats = editMovieViewModel.PopulateList(_context.MediaFormats.ToList());
            editMovieViewModel.AudioFormats = editMovieViewModel.PopulateList(_context.AudioFormats.ToList());
            editMovieViewModel.AvailableGenres = _context.Genres.ToList();
            return View(editMovieViewModel);
        }

        private async Task<int> UpdateMovieAsync(AddMovieViewModel viewModel, Film film)
        {   
            // UpdateMovie transfers data from viewModel to the Film object.  The created
            // or edited film's ID is returned as an int.

            MediaFormat newMediaFormat = _context.MediaFormats.Single(m => m.ID == viewModel.MediaID);
            AudioFormat newAudioFormat = _context.AudioFormats.Single(a => a.ID == viewModel.AudioID);

            film.Name = viewModel.Name;
            film.Year = viewModel.Year;
            film.AspectRatio = viewModel.AspectRatio;
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
            film.Runtime = viewModel.Runtime;

            // Adds the film to the db if it does not already exist.
            Film existingFilm = _context.Films.SingleOrDefault(f => f.ID == film.ID);
            if (existingFilm == null)
            {
                _context.Films.Add(film);
            }

            // Add genres to the film.  Deletes any previously selected genres if they arne't still selected from an edit.
            if (viewModel.Genres != null)
            {
                List<FilmGenre> thisFilmGenres = _context.FilmGenres.Include(fg => fg.Genre).Where(fg => fg.FilmID == film.ID).ToList();

                // Eliminate any previous FilmGenres that weren't selected in an edit
                foreach (FilmGenre filmGenre in thisFilmGenres)
                {
                    if (!viewModel.Genres.Contains(filmGenre.Genre.Name))
                    {
                        _context.FilmGenres.Remove(filmGenre);
                    }
                }

                // Add selected Genres to a film <FilmGenre> if it doesn't already exist
                foreach (string genre in viewModel.Genres)
                {
                    // Gets a list of existing FilmGenre items to check against.
                    IList<FilmGenre> existingFilmGenres = _context.FilmGenres.Where(fg => fg.FilmID == film.ID)
                        .Where(fg => fg.Genre.Name == genre).ToList();

                    Models.Genre newGenre = _context.Genres.Single(g => g.Name == genre);

                    // If no FilmGenres with the current FilmID and GenreID exist, one is created.
                    if (existingFilmGenres.Count == 0)
                    {
                        FilmGenre newFilmGenre = new FilmGenre
                        {
                            FilmID = film.ID,
                            GenreID = newGenre.ID
                        };

                        _context.FilmGenres.Add(newFilmGenre);
                    }
                    
                }
            }

            // The time the Film and User are updated are saved, allowing them to be sorted later
            // by most recent activity.
            film.Updated = DateTime.Now;
            ApplicationUser user = await _userManager.GetUserAsync(User);
            user.Updated = DateTime.Now;
            _context.SaveChanges();

            return film.ID;
        }

        [NonAction]
        private List<string> GetGenres(int filmId)
        {
            // Retrieves a list of FilmGenres associated with a film and returns a
            // List of strings containing the names of the Genres.  These are the
            // Genres that have previously been selected for the Film.
            List<string> genres = new List<string>();
            List<FilmGenre> FilmGenres = _context.FilmGenres.Include(fg => fg.Genre).Where(fg => fg.FilmID == filmId).ToList();
            foreach (var filmGenre in FilmGenres)
            {
                genres.Add(filmGenre.Genre.Name);
            }
            return genres;
        }

        [NonAction]
        private SearchContainer<SearchMovie> TMDbSearch(TMDbClient client, string query, int page)
        {
            // Attempts to search TMDb for movies matching the title submitted (query).
            // If successful, the results are returned.  If there is an exception, Null
            // is returned.

            try
            {
                SearchContainer<SearchMovie> results = client.SearchMovieAsync(query, page).Result;
                return results;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [NonAction]
        private Movie GetTMDbMovieInfo(int Id)
        {
            // Attempts to search TMDb for a Movie object matching the Id submitted.
            // If successful, the results are returned.  If there is an exception, Null
            // is returned.

            try
            {
                Movie movie = client.GetMovieAsync(Id, MovieMethods.Credits | MovieMethods.Videos | MovieMethods.Images).Result;
                return movie;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [NonAction]
        private List<Film> GetFilmsByTitle(string title)
        {
            // Returns all Films, containing search parameter, that the current User is authorized to see.  
            // Audio, Media, FilmGenre.Genre and User of the film are included.
            IQueryable<Film> films = _context.Films
                .Include(f => f.Audio)
                .Include(f => f.Media)
                .Include(f => f.User)
                .Include(f=> f.FilmGenres)
                    .ThenInclude(fg=>fg.Genre)
                .Where(f => f.Name.ToLower().Contains(title.ToLower())
                && ((!f.IsPrivate && !f.User.IsPrivate) || User.IsInRole("Admin") || f.UserID == _userManager.GetUserId(User)));
            return films.ToList();
        }

        [NonAction]
        private List<Film> GetAllFilmsByGenre(string title, string genre = null)
        {
            // Returns all Films, containing the search parameter, for a specific genre, 
            // displaying only those that the current User is authorized to see 
            return GetFilmsByTitle(title).Where(f => f.FilmGenres.Any(fg => fg.Genre.Name == genre)).ToList();
        }

        [NonAction]
        private List<Film> GetAllFilmsByMediaFormat(string title, string mediaFormat = null)
        {
            // Returns all Films, containing the search parameter, for a specific MediaFormat, 
            // displaying only those that the current User is authorized to see 
            return GetFilmsByTitle(title).Where(f=> f.Media.Name == mediaFormat).ToList();
        }

        [NonAction]
        private List<Film> GetAllFilmsByAudioFormat(string title, string audioFormat = null)
        {
            // Returns all Films, containing the search parameter, for a specific AudioFormat, 
            // displaying only those that the current User is authorized to see 

            return GetFilmsByTitle(title).Where(f => f.Audio.Name == audioFormat).ToList();
        }

        
    }


}
