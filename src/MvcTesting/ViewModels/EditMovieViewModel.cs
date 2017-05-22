using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.Movies;

namespace MvcTesting.ViewModels
{
    public class EditMovieViewModel : AddMovieViewModel
    {
        public int ID { get; set; }

        public EditMovieViewModel() { }

        public EditMovieViewModel(IEnumerable<MediaFormat> mediaFormats, IEnumerable<AudioFormat> audioFormats, Film film) : base(mediaFormats, audioFormats)
        {
            List<string> directors = new List<string>();
            List<string> cast = new List<string>();

            SetRatings();
            AudioFormats = PopulateList(audioFormats);
            MediaFormats = PopulateList(mediaFormats);
            Name = film.Name;
            TMDbId = film.TMDbId;
            Overview = film.Overview;

            if (!string.IsNullOrEmpty(film.Year))
            {
                Year = "(" + film.Year + ")";
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
            
        }
    }

    
}
