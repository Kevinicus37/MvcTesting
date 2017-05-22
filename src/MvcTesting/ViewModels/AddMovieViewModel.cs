using Microsoft.AspNetCore.Mvc.Rendering;
using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;

namespace MvcTesting.ViewModels
{
    public class AddMovieViewModel
    {
        // Title of Movie/Disc
        [Required]
        [Display(Name = "Title (Required):")]
        public string Name { get; set; }

        // Format (DVD, Blu-ray, 4K UHD, Digital, etc.)
        [Required]
        [Display(Name="Disc Format (Required):")]
        public int MediaID { get; set; }

        [RegularExpression("^([1][9][0-9]{2}|[2-9][0-9]{3})$", ErrorMessage = "Year must be a number greater than 1900")]
        [Display(Name = "Year:")]
        public string Year { get; set; }

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
        [Range(0,10)]
        public int Rating { get; set; }

        // Director(s) of the movie
        [Display(Name = "Director(s) - Separate multiple Directors by comma:")]
        public string Directors { get; set; }

        // Cast Members
        [Display(Name = "Top Cast Members - separate by comma:")]
        public string Cast { get; set; }

        // Audio Format (Dolby Digital, DTS, Dolby True HD, DTS-MA HD, Dolby Atmos, DTS-X, Auro, Stereo, Mono, etc.)
        // Likely need to create a class
        [Required]
        [Display(Name = "Audio Format (Required):")]
        public int AudioID { get; set; }

        // A general description of what the movie is about.
        [Display(Name = "Overview:")]
        public string Overview { get; set; }

        // Genres the film belongs to.
        // Likely need to create a class
        [Display(Name = "Genres:")]
        public List<string> Genres { get; set; }

        // Url for the film's trailer on Youtube.com
        [Display(Name = "Trailer URL:")]
        public string TrailerUrl { get; set; }

        // Url for an image of the film's poster.
        [Display(Name = "Poster URL:")]
        public string PosterUrl { get; set; }

        [Display(Name="3D Option?")]
        public bool Has3D { get; set; }

        [Display(Name="Private? (If Checked, others will not be able to see this movie in your collection.)")]
        public bool IsPrivate { get; set; }


        public List<SelectListItem> Ratings { get; set; }
        public List<SelectListItem> MediaFormats { get; set; }
        public List<SelectListItem> AudioFormats { get; set; }

        public AddMovieViewModel()
        {
            SetRatings();
        }

        public AddMovieViewModel(IEnumerable<MediaFormat> mediaFormats, IEnumerable<AudioFormat> audioFormats)
        {
            SetRatings();
            AudioFormats=PopulateList<IEnumerable<AudioFormat>>(audioFormats);
            MediaFormats = PopulateList<IEnumerable<MediaFormat>>(mediaFormats);
            
        }

        public AddMovieViewModel(IEnumerable<MediaFormat> mediaFormats, IEnumerable<AudioFormat> audioFormats, Movie movie)
        {
            List<string> directors = new List<string>();
            List<string> cast = new List<string>();

            SetRatings();
            AudioFormats = PopulateList(audioFormats);
            MediaFormats = PopulateList(mediaFormats);
            Name = movie.Title;
            TMDbId = movie.Id;
            Overview = movie.Overview;

            if (movie.ReleaseDate.Value != null)
            {
                Year = "(" + movie.ReleaseDate.Value.ToString("yyyy") + ")";
            }

            
            if (movie.Videos.Results.Count > 0)
            {
                TrailerUrl = "https://www.youtube.com/embed/" + movie.Videos.Results[movie.Videos.Results.Count - 1].Key;
            }
            
            if (movie.Images.Posters.Count > 0)
            {
                PosterUrl = "https://image.tmdb.org/t/p/w300_and_h450_bestv2/" + movie.Images.Posters[0].FilePath;
            }

            if (movie.Credits.Crew.Count > 0)
            {
                foreach (Crew member in movie.Credits.Crew)
                {
                    if (member.Job == "Director")
                    {
                        directors.Add(member.Name);
                    }
                }
            }
            Directors = String.Join(", ", directors);

            for (int i = 0; i < movie.Credits.Cast.Count && i <= 8; i++)
            {
                cast.Add(movie.Credits.Cast[i].Name);
            }
            Cast = String.Join(", ", cast);
        }
        
        public void SetRatings()
        {
            Ratings = new List<SelectListItem>();

            Ratings.Add(new SelectListItem { Value = 0.ToString(), Text = "No Rating." });

            for (int i = 1; i <= 10; i++)
            {
                Ratings.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }

        }

        public List<SelectListItem> PopulateList<T>(T items) where T : IEnumerable<IItemList>
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            foreach (var item in items)
            {
                Items.Add(new SelectListItem
                {
                    Value = item.ID.ToString(),
                    Text = item.Name
                });
            }

            return Items;
        }
    }
}