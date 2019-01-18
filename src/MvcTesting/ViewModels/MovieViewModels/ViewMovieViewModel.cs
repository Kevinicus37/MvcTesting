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

        public string OwnerProfilePicture { get; set; } = "/images/profilePictureDefault.png";

        public int OwnerCollectionSize { get; set; }

        public string Directors { get; set; }

        public List<string> CastMembers { get; set; } = new List<string>();

        public string RunTime { get; set; }

        public List<FilmGenre> Genres { get; set; } = new List<FilmGenre>();

        public string OptionFor3D { get; set; } = "No";

        public string Comments { get; set; }

        public string MediaFormat { get; set; }

        public string AudioFormat { get; set; }

        public string PosterUrl { get; set; } = "/images/filmposterdefault.jpg";

        public ViewMovieViewModel(Film film, List<FilmGenre> genres)
        {
            Film = film;

            Directors = film.Directors;
            RunTime = film.Runtime.ToString();
            Comments = film.Comments;
            MediaFormat = film.Media.Name;
            AudioFormat = film.Audio.Name;
            FilmOwnerId = film.UserID;
                
            if (!string.IsNullOrEmpty(film.Cast))
            {
                CastMembers = film.Cast.Split(',').ToList();
            }

            if (film.Has3D)
            {
                OptionFor3D = "Yes";
            }

            Genres = genres;

            if (film.Year != null)
            {
                DisplayYear = "(" + film.Year + ")";
            }

            if (!string.IsNullOrEmpty(film.PosterUrl))
            {
                PosterUrl = film.PosterUrl;
            }
            
        }
    }

    
}
