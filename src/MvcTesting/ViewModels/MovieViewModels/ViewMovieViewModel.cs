using MvcTesting.Models;
using System.Collections.Generic;
using System.Linq;

namespace MvcTesting.ViewModels
{
    public class ViewMovieViewModel
    {
        public Film Film { get; set; }

        public IList<string> CastMembers { get; set; }

        public IList<FilmGenre> Genres { get; set; } 

        public string DisplayYear { get; set; }

        public ViewMovieViewModel(Film film, IList<FilmGenre> genres)
        {
            Film = film;
            CastMembers = new List<string>();
            if (!string.IsNullOrEmpty(film.Cast)) CastMembers = film.Cast.Split(',').ToList();
            Genres = genres;
            if (film.Year != null)
            {
                DisplayYear = "(" + film.Year + ")";
            }
        }
    }

    
}
