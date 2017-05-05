using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.Movies;

namespace MvcTesting.ViewModels
{
    public class AddMovieViewModel
    {
        // Title of Movie/Disc
        [Required]
        [Display(Name = "Title:")]
        public string Name { get; set; }

        // Format (DVD, Blu-ray, 4K UHD, Digital, etc.)
        [Required]
        [Display(Name="Disc Format:")]
        public string Format { get; set; }


        [Display(Name = "Year:")]
        [DisplayFormat(ApplyFormatInEditMode =true, DataFormatString ="{0:####}")]
        public int Year { get; set; }

        // Aspect ratio of film (1.33:1, 1.77:1, 1.85:1, 16:9, 2.35:1, 2.39:1, 2.4:1, shifting, etc.)
        [Display(Name = "Aspect Ratio:")]
        public string AspectRatio { get; set; }

        // Film Id on TMDb.org
        [Display(Name = "TMDb.org Film ID:")]
        public int TMDbId { get; set; }

        // Thoughts a user has on the movie
        [Display(Name = "Comments:")]
        public string Comments { get; set; }

        // Rating (1-10) that the user gives the movie.
        [Display(Name = "Rating (1-10)")]
        [Range(1,10)]
        public int Rating { get; set; }

        // Director(s) of the movie
        [Display(Name = "Director(s):")]
        public IList<string> Directors { get; set; }

        // Cast Members
        [Display(Name = "Cast (separate by ','):")]
        public IList<string> Cast { get; set; }

        // Audio Format (Dolby Digital, DTS, Dolby True HD, DTS-MA HD, Dolby Atmos, DTS-X, Auro, Stereo, Mono, etc.)
        [Display(Name = "Audio Format:")]
        public string AudioFormat { get; set; }

        // A general description of what the movie is about.
        [Display(Name = "Overview:")]
        public string Overview { get; set; }

        // Distributor of the film
        [Display(Name = "Distributor:")]
        public string Distributor { get; set; }

        // Genres the film belongs to.
        [Display(Name = "Genres:")]
        public IList<string> Genres { get; set; }

        // Url for the film's trailer on Youtube.com
        [Display(Name = "Trailer URL:")]
        public string TrailerUrl { get; set; }

        // Url for an image of the film's poster.
        [Display(Name = "Poster URL:")]
        public string PosterUrl { get; set; }

        public List<SelectListItem> Ratings
        { get; set; }


        public AddMovieViewModel()
        {
            SetRatings();            
        }

        public AddMovieViewModel(Movie movie)
        {
            SetRatings();
            Name = movie.Title;
            TMDbId = movie.Id;
            Overview = movie.Overview;

            if (movie.ReleaseDate.Value != null)
            {
                Year = int.Parse(movie.ReleaseDate.Value.ToString("yyyy"));
            }

            
            if (movie.Videos.Results.Count > 0)
            {
                TrailerUrl = "https://www.youtube.com/embed/" + movie.Videos.Results[movie.Videos.Results.Count - 1].Key;
            }

            if (movie.Images.Posters.Count > 0)
            {
                PosterUrl = "https://image.tmdb.org/t/p/w300_and_h450_bestv2/" + movie.Images.Posters[0];
            }
        }

        public void SetRatings()
        {
            Ratings = new List<SelectListItem>();

            for (int i = 1; i <= 10; i++)
            {
                Ratings.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }

        }
    }
}