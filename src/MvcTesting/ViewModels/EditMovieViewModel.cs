using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class EditMovieViewModel : AddMovieViewModel
    {
        public int ID { get; set; }

        public EditMovieViewModel() { }

        public EditMovieViewModel(IEnumerable<MediaFormat> mediaFormats, IEnumerable<AudioFormat> audioFormats, Film film) : base(mediaFormats, audioFormats, null)
        {
            List<string> directors = new List<string>();
            List<string> cast = new List<string>();

            Name = film.Name;
            TMDbId = film.TMDbId;
            Overview = film.Overview;

            Year = film.Year;
            if (!string.IsNullOrEmpty(Year))
            {
                DisplayYear = "(" + Year + ")";
            }

            TrailerUrl = film.TrailerUrl;
            PosterUrl = film.PosterUrl;
            Directors = film.Directors;
            Cast = film.Cast;
            Has3D = film.Has3D;
            IsPrivate = film.IsPrivate;
            Rating = film.Rating;
            AudioID = film.AudioID;
            MediaID = film.MediaID;
            Comments = film.Comments;
            AspectRatio = film.AspectRatio;
            Runtime = film.Runtime;
            
        }
    }

    
}
