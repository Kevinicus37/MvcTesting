using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels.MovieViewModels
{
    public class CopyMovieViewModel : AddMovieViewModel
    {
        public CopyMovieViewModel() { }

        public CopyMovieViewModel(IEnumerable<MediaFormat> mediaFormats, IEnumerable<AudioFormat> audioFormats, Film film) : base(mediaFormats, audioFormats, null)
        {
            List<string> directors = new List<string>();
            List<string> cast = new List<string>();

            Name = film.Name;
            Overview = film.Overview;

            Year = film.Year;
            if (Year != null)
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
