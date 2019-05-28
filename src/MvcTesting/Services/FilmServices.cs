using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcTesting.Services
{
    public class FilmServices
    {
        private readonly MovieCollectorContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FilmServices(MovieCollectorContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        
        public IQueryable<Film> GetAllCompleteFilms()
        {
            return _context.Films
                .Include(f => f.User)
                .Include(f => f.Media)
                .Include(f => f.Audio)
                .Include(f => f.FilmGenres)
                    .ThenInclude(fg => fg.Genre)
                .OrderBy(f => f.Name)
                .OrderByDescending(f => f.Updated);
        }

        public List<Film> GetAllVisible(List<Film> films, ClaimsPrincipal appUser)
        {
            return films
                .Where(f => (!f.IsPrivate && !f.User.IsPrivate) || appUser.IsInRole("Admin") || f.UserID == _userManager.GetUserId(appUser))
                .ToList();
        }

        public List<Film> SortBy(List<Film> films, SortPriority sortPriority)
        {
            // Sorts the List<Film> based upon the sortValue
            // TODO: Find a way to eliminate the magic strings

            switch (sortPriority)
            {
                case SortPriority.Title:
                    return SortByTitle(films);
                case SortPriority.TitleDesc:
                    return SortByTitleDescending(films);
                case SortPriority.Year:
                    return SortByYear(films);
                case SortPriority.YearDesc:
                    return SortByYearDescending(films);
                case SortPriority.Media:
                    return SortByMediaFormat(films);
                case SortPriority.MediaDesc:
                    return SortByMediaFormatDescending(films);
                case SortPriority.Audio:
                    return SortByAudioFormat(films);
                case SortPriority.AudioDesc:
                    return SortByAudioFormatDescending(films);
                default:
                    return SortByTitle(films);
            }
        }

        public List<Film> SortByTitle(List<Film> films)
        {
            return films.OrderBy(f => f.Name).ToList();
        }

        public List<Film> SortByTitleDescending(List<Film> films)
        {
            return films.OrderByDescending(f => f.Name).ToList();
        }

        public List<Film> SortByYear(List<Film> films)
        {
            return films.OrderBy(f => f.Year).ToList();
        }

        public List<Film> SortByYearDescending(List<Film> films)
        {
            return films.OrderByDescending(f => f.Year).ToList();
        }

        public List<Film> SortByAudioFormat(List<Film> films)
        {
            return films.OrderBy(f => f.Audio.Name).ToList();
        }

        public List<Film> SortByAudioFormatDescending(List<Film> films)
        {
            return films.OrderByDescending(f => f.Audio.Name).ToList();
        }

        public List<Film> SortByMediaFormat(List<Film> films)
        {
            return films.OrderBy(f => f.Media.Name).ToList();
        }

        public List<Film> SortByMediaFormatDescending(List<Film> films)
        {
            return films.OrderByDescending(f => f.Media.Name).ToList();
        }

        public bool IsFilmViewable(Film film, ClaimsPrincipal user)
        {
            if (!film.IsPrivate && film.User != null && !film.User.IsPrivate)
            {
                return true;
            }
            else if (_userManager.GetUserId(user) == film.UserID)
            {
                return true;
            }
            else if (user.IsInRole("Admin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateFilmGenres(List<string> genreIds, int filmId)
        {
            foreach (string genre in genreIds)
            {
                // Gets a list of existing FilmGenre items to check against.
                IList<FilmGenre> existingFilmGenres = _context.FilmGenres
                    .Where(fg => fg.FilmID == filmId && fg.Genre.ID.ToString() == genre)
                    .ToList();

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

        public List<string> GetFilmGenreIds(int filmId)
        {
            // Retrieves a list of FilmGenres associated with a film and returns a
            // List of strings containing the IDs of those Genres.  These are the
            // Genres that have previously been selected for the Film.
            List<string> genres = _context.FilmGenres.Where(x => x.FilmID == filmId).Select(x => x.GenreID.ToString()).ToList();

            return genres;
        }
    }
}
