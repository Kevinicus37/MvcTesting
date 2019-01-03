using MvcTesting.Models;
using System.Collections.Generic;
using System.Linq;

namespace MvcTesting.ViewModels
{
    public class ViewMovieViewModel
    {
        public Film Film { get; set; }

        public string DisplayYear { get; set; }

        public string FilmOwnerName { get; set; }

        public string FilmOwnerId { get; set; }

        public string OwnerProfilePicture { get; set; }

        public int OwnerCollectionSize { get; set; }

        public string Directors { get; set; }

        public List<string> CastMembers { get; set; } = new List<string>();

        public string RunTime { get; set; }

        public List<FilmGenre> Genres { get; set; } = new List<FilmGenre>();

        public bool Has3D {get; set;}

        public string Comments { get; set; }

        public string MediaFormat { get; set; }

        public string AudioFormat { get; set; }

        public ViewMovieViewModel(Film film, List<FilmGenre> genres)
        {
            Film = film;

            Directors = film.Directors;
            RunTime = film.Runtime.ToString();
            Has3D = film.Has3D;
            Comments = film.Comments;
            MediaFormat = film.Media.Name;
            AudioFormat = film.Audio.Name;
            FilmOwnerId = film.UserID;
                
            if (!string.IsNullOrEmpty(film.Cast))
            {
                CastMembers = film.Cast.Split(',').ToList();
            }

            Genres = genres;

            if (film.Year != null)
            {
                DisplayYear = "(" + film.Year + ")";
            }

            
        }
    }

    
}
