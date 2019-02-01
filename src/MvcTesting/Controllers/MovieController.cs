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
using MvcTesting.ViewModels.MovieViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            // Gets a list of all films in order of Name, and then by Year and displays it in the view.

            List<Film> films = _context.Films.Include(f=>f.User).OrderBy(f => f.Name)
                .Where(f => (!f.IsPrivate && !f.User.IsPrivate) || f.UserID == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                .OrderByDescending(f => f.Updated)
                .ToList();

            MovieIndexViewModel movieIndexViewModel = new MovieIndexViewModel(films);

            List<ApplicationUser> users = _context.Users.Include(u => u.Films).Where(u => u.IsPrivate == false)
                .Where(u => u.Films.Count > 0)
                .OrderByDescending(u => u.Films.Count)
                .Take(15)
                .ToList();

            movieIndexViewModel.Users = users;

            return View(movieIndexViewModel);
        }

        public IActionResult WebSearch()
        {
            // Displays view allowing a user to search for a movie (using TMDb) to add to their collection.

            return View();
        }

        [HttpPost]
        public IActionResult WebSearch(string query, int page)
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
        public IActionResult Search(string query)
        {
            
            List<Film> films = _context.Films
                .Include(f=>f.User)
                .Where(f => (!f.IsPrivate && !f.User.IsPrivate) || User.IsInRole("Admin") || f.UserID == _userManager.GetUserId(User))
                .Where(f => f.Name.ToLower().Contains(query.ToLower()))
                .ToList();

            var vm = new SearchMovieViewModel(films);

            vm.Genres = _context.Genres.ToList();
            vm.MediaFormats = _context.MediaFormats.ToList();
            vm.AudioFormats = _context.AudioFormats.ToList();
            
            
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Search(SearchMovieViewModel vm)
        {
            // TODO - Show what filters have been applied.
            List<Film> films = new List<Film>();
            
            // TODO - Create a WhereIf() extension method.
            // Gets films matching the search value and filter (if selected).
            films = _context.Films
                .Include(f => f.Audio)
                .Include(f => f.Media)
                .Include(f => f.FilmGenres).ThenInclude(fg => fg.Genre)
                .Include(f=>f.User)
                .ToList();
                    
            films=films.Where(f=> string.IsNullOrEmpty(vm.SearchValue) || f.Name.ToLower().Contains(vm.SearchValue.ToLower()))
                .Where(f => (!f.IsPrivate && !f.User.IsPrivate) || User.IsInRole("Admin") || f.UserID == _userManager.GetUserId(User))
                .Where(f => string.IsNullOrEmpty(vm.AudioFilter) || f.Audio.Name == vm.AudioFilter)
                .Where(f => string.IsNullOrEmpty(vm.MediaFilter) || f.Media.Name == vm.MediaFilter)
                .Where(f => string.IsNullOrEmpty(vm.GenreFilter) || f.FilmGenres.Any(fg => fg.Genre != null && fg.Genre.Name == vm.GenreFilter))
                .ToList();
            
            // sort the collection of films by selected value (title ascending is default)
            vm.Films = films.SortByValue(vm.SortValue);

            // TODO - Check if Genres needs the full Genre models or just the Ids
            vm.Genres = _context.Genres.ToList();
            vm.MediaFormats = _context.MediaFormats.ToList();
            vm.AudioFormats = _context.AudioFormats.ToList();


            // TODO - Add pagination for the results.  See WebSearch.
            return View(vm);
        }

        public IActionResult ViewSearchedMovie(int Id)
        {
            // TODO - Create View Model to help display movie.  Do proper trailer selection.
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
            addMovieViewModel.SetGenres(_context, movie);
            addMovieViewModel.AvailableGenres = _context.Genres
                .Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name, Selected = addMovieViewModel.Genres.Contains(x.ID.ToString())})
                .ToList();
            
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
            // TODO - Check if this maintains privacy settings.

            // Finds and displays a Film based on the ID.  If no Film is found with a matching ID,
            // the User is returned to the Index action.
            Film film = _context.Films.Include(f => f.Media).Include(f => f.Audio).SingleOrDefault(f => f.ID == id);
            if (film != null)
            {
                
                List<FilmGenre> genres = _context.FilmGenres.Include(g => g.Genre).Where(f => f.FilmID == id).ToList();
                ViewMovieViewModel viewMovieViewModel = new ViewMovieViewModel(film, genres);

                ApplicationUser user = _context.Users.Where(u => u.Id == film.UserID).Include(u=> u.Films).FirstOrDefault();
                if (user != null)
                {
                    viewMovieViewModel.FilmOwnerName = user.UserName;
                    viewMovieViewModel.OwnerCollectionSize = user.Films.Count;

                    if (!string.IsNullOrEmpty(user.ProfilePicture))
                    {
                        viewMovieViewModel.OwnerProfilePicture = $"/images/{user.UserName}/{user.ProfilePicture}";
                    }
                }

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
                    .SingleOrDefault(f => f.ID == id);
                
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

                List<MediaFormat> mediaFormats = _context.MediaFormats.ToList();
                List<AudioFormat> audioFormats = _context.AudioFormats.ToList();
                EditMovieViewModel editMovieViewModel = new EditMovieViewModel(mediaFormats, audioFormats, editFilm);
                editMovieViewModel.Genres = GetFilmGenreIds(id);
                editMovieViewModel.AvailableGenres = _context.Genres
                    .Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name, Selected = editMovieViewModel.Genres.Contains(x.ID.ToString()) })
                    .ToList();

                if (editFilm.UserID == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    editMovieViewModel.ID = id;
                }

                return View(editMovieViewModel);
                
            }
            
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMovieViewModel editMovieViewModel)
        {
            // Verify model data is valid and that the active User is the owner of the Film
            // or Admin.  If so, the Film is then updated.  The User is Redirected to the page of
            // the Film.  
            
            if (ModelState.IsValid)
            {

                Film film = _context.Films.Single(f => f.ID == editMovieViewModel.ID);
                if (!(film.UserID == _userManager.GetUserId(User) || User.IsInRole("Admin")))
                {
                    film = new Film();
                    editMovieViewModel.ID = 0;
                }
                    int id = await UpdateMovieAsync(editMovieViewModel, film);
                    return Redirect($"/Movie/ViewMovie/{id}");
                //}
                
                //return RedirectToAction("Index");
            }

            // If the model is not valid, it is re-seeded and returned to the View.
            editMovieViewModel.MediaFormats = editMovieViewModel.PopulateList(_context.MediaFormats.ToList());
            editMovieViewModel.AudioFormats = editMovieViewModel.PopulateList(_context.AudioFormats.ToList());
            editMovieViewModel.AvailableGenres = _context.Genres
                .Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name, Selected = editMovieViewModel.Genres.Contains(x.ID.ToString()) })
                .ToList();

            return View(editMovieViewModel);
        }

        public IActionResult Copy(int id)
        {
            Film editFilm = _context.Films.SingleOrDefault(f => f.ID == id);

            // If editFilm with the passed in id exists and the User is associated with the Film or Admin,
            // then the ViewModel is seeded.  Otherwise the User is redirected to the Index Action.
            if (editFilm != null)
            {

                List<MediaFormat> mediaFormats = _context.MediaFormats.ToList();
                List<AudioFormat> audioFormats = _context.AudioFormats.ToList();
                CopyMovieViewModel vm = new CopyMovieViewModel(mediaFormats, audioFormats, editFilm);
                vm.Genres = GetFilmGenreIds(id);
                vm.AvailableGenres = _context.Genres
                    .Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name, Selected = vm.Genres.Contains(x.ID.ToString()) })
                    .ToList();

                return View(vm);

            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Copy(CopyMovieViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Film film = new Film();
                vm.ID = 0;
                
                int id = await UpdateMovieAsync(vm, film);
                return Redirect($"/Movie/ViewMovie/{id}");
                //}

                //return RedirectToAction("Index");
            }

            // If the model is not valid, it is re-seeded and returned to the View.
            vm.MediaFormats = vm.PopulateList(_context.MediaFormats.ToList());
            vm.AudioFormats = vm.PopulateList(_context.AudioFormats.ToList());
            vm.AvailableGenres = _context.Genres
                .Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name, Selected = vm.Genres.Contains(x.ID.ToString()) })
                .ToList();

            return View(vm);
        }


        // Additional Helper Functions

        private async Task<int> UpdateMovieAsync(AddMovieViewModel viewModel, Film film)
        {
            // UpdateMovie transfers data from viewModel to the Film object.  The created
            // or edited film's ID is returned as an int.
            ApplicationUser user = await _userManager.GetUserAsync(User);
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
            if (!string.IsNullOrEmpty(viewModel.PosterUrl))
            {
                film.PosterUrl = viewModel.PosterUrl;
            }
            else
            {
                film.PosterUrl = "/images/filmposterdefault.jpg";
            }
            
            film.IsPrivate = viewModel.IsPrivate;
            film.Has3D = viewModel.Has3D;
            film.Audio = newAudioFormat;
            film.Media = newMediaFormat;
            film.Runtime = viewModel.Runtime;

            // Adds the film to the db if it does not already exist.
            Film existingFilm = _context.Films.SingleOrDefault(f => f.ID == film.ID);
            if (existingFilm == null)
            {
                film.UserID = user.Id;
                _context.Films.Add(film);
            }

            // Add genres to the film.  Deletes any previously selected genres if they arne't still selected from an edit.
            if (viewModel.Genres != null)
            {
                EraseGenres(viewModel, film.ID);
                CreateFilmGenres(viewModel, film.ID);
            }

            // The time the Film and User are updated are saved, allowing them to be sorted later
            // by most recent activity.
            film.Updated = DateTime.Now;
            user.Updated = DateTime.Now;
            _context.SaveChanges();

            return film.ID;
        }
        
        [NonAction]
        private void EraseGenres(AddMovieViewModel vm, int filmId)
        {
            // Deletes any existing FilmGenres that are no longer selected for this film.

            List<FilmGenre> thisFilmGenres = _context.FilmGenres.Include(fg => fg.Genre).Where(fg => fg.FilmID == filmId).ToList();

            // Eliminate any previous FilmGenres that weren't selected in an edit
            foreach (FilmGenre filmGenre in thisFilmGenres)
            {
                if (!vm.Genres.Contains(filmGenre.Genre.ID.ToString()))
                {
                    _context.FilmGenres.Remove(filmGenre);
                }
            }
        }

        [NonAction]
        private void CreateFilmGenres(AddMovieViewModel vm, int filmId)
        {
            foreach (string genre in vm.Genres)
            {
                // Gets a list of existing FilmGenre items to check against.
                IList<FilmGenre> existingFilmGenres = _context.FilmGenres.Where(fg => fg.FilmID == filmId)
                    .Where(fg => fg.Genre.ID.ToString() == genre).ToList();

                // If no FilmGenres with the current FilmID and GenreID exist, one is created.
                if (existingFilmGenres.Count == 0)
                {
                    FilmGenre newFilmGenre = new FilmGenre
                    {
                        FilmID = filmId,
                        GenreID = int.Parse(genre)
                    };

                    _context.FilmGenres.Add(newFilmGenre);
                }
            }
        }
        
        [NonAction]
        private List<string> GetFilmGenreIds(int filmId)
        {
            // Retrieves a list of FilmGenres associated with a film and returns a
            // List of strings containing the IDs of those Genres.  These are the
            // Genres that have previously been selected for the Film.
            List<string> genres = new List<string>();
            List<FilmGenre> FilmGenres = _context.FilmGenres.Include(fg => fg.Genre).Where(fg => fg.FilmID == filmId).ToList();
            foreach (var filmGenre in FilmGenres)
            {
                genres.Add(filmGenre.Genre.ID.ToString());
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
            catch (Exception e) // Catches and logs generic exception if TMDb cannot be reached for any reason.
            {
                Console.Out.WriteLine(e.Message);
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
            catch (Exception e)  // Catches and logs generic exception if TMDb cannot be reached for any reason.
            {
                Console.Out.WriteLine(e.Message);
                return null;
            }
        }
    }
}
