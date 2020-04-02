using Microsoft.AspNetCore.Mvc.Rendering;
using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        // Release year of the Movie object.
        [Range(1900, 9999)]
        [Display(Name = "Year:")]
        public int? Year { get; set; }

        // Aspect ratio of film (1.33:1, 1.77:1, 1.85:1, 16:9, 2.35:1, 2.39:1, 2.4:1, shifting, etc.)
        [Display(Name = "Aspect Ratio:")]
        public string AspectRatio { get; set; }

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
        [Required]
        [Display(Name = "Audio Format (Required):")]
        public int AudioID { get; set; }

        // A general description of what the movie is about.
        [Display(Name = "Overview:")]
        public string Overview { get; set; }

        // Genres the film belongs to.
        [Display(Name = "Genres:")]
        public List<string> Genres { get; set; } = new List<string>();

        // Available Genres a film can belong to.
        public List<SelectListItem> AvailableGenres { get; set; } = new List<SelectListItem>();

        // Url for the film's trailer on Youtube.com
        [Display(Name = "Trailer URL:")]
        public string TrailerUrl { get; set; }

        // Url for an image of the film's poster.
        [Display(Name = "Poster URL:")]
        public string PosterUrl { get; set; }

        // Is the media capable of 3D playback?
        [Display(Name="3D Option?")]
        public bool Has3D { get; set; }

        // Set the movie to private so that it can only be seen by the User or Admin?
        [Display(Name="Private? (If Checked, others will not be able to see this movie in your collection.)")]
        public bool IsPrivate { get; set; }

        // Length of the Movie in minutes
        [Display(Name= "Runtime (in minutes):")]
        [Range(1,999)]
        public int? Runtime { get; set; }

        // String used to display the year with parenthesese.  
        public string DisplayYear { get; set; }

        public List<SelectListItem> Ratings { get; set; }
        public List<SelectListItem> MediaFormats { get; set; }
        public List<SelectListItem> AudioFormats { get; set; }

        public AddMovieViewModel()
        {
            SetRatings();
        }
        
        public AddMovieViewModel(IEnumerable<MediaFormat> mediaFormats, IEnumerable<AudioFormat> audioFormats, Movie movie)
        {
            
            // Populates the options for Rating, AudioFormat, and MediaFormat
            SetRatings();
            AudioFormats = PopulateList(audioFormats);
            MediaFormats = PopulateList(mediaFormats);
            
            if (movie != null)
            {
                Name = movie.Title;
                Overview = movie.Overview;
                Runtime = movie.Runtime;

                FormatDisplayedDate(movie);
                SetTrailer(movie);
                SetPoster(movie);
                SetDirectors(movie);
                SetCast(movie);
            }
        }
        
        public void SetRatings()
        {

            Ratings = new List<SelectListItem>();

            Ratings.Add(new SelectListItem { Value = "0", Text = "No Rating." });

            for (int i=1; i<=10; i++)
            {
                Ratings.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }

        }

        public List<SelectListItem> PopulateList<T>(T items) where T : IEnumerable<MediaProperty>
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

        public void SetGenres(MovieCollectorContext _context, Movie movie)
        {
            // TODO - Remove usage of dbContext in the view model
            // Add selected genres from tmdb.org
            foreach (var genre in movie.Genres)
            {
                Genres.Add(_context.Genres.Where(x => x.Name == genre.Name).First().ID.ToString());
            }

            

        }

        public void FormatDisplayedDate(Movie movie)
        {
            // If the Movie object has release year data, it
            // is bracketed by parenthese for the DisplayYear string.
            if (movie.ReleaseDate != null)
            {
                Year = movie.ReleaseDate.Value.Year;
                DisplayYear = "(" + Year + ")";
            }

        }

        public void SetTrailer(Movie movie)
        {
            var videos = movie.Videos.Results;
            // If there are videos associated with the movie by tmdb.org
            // the key for the last one listed as type 'Trailer' is added
            // to the youtube address to get the URL for the trailer.
            if (videos.Count > 0)
            {
                string key = videos[0].Key;

                // Find the last results of type of "Trailer" to get the best choice.
                for (int i = videos.Count - 1; i >= 0; i--)
                {

                    if (videos[i].Type == "Trailer")
                    {
                        key = videos[i].Key;
                        break;
                    }
                }

                TrailerUrl = GlobalVariables.BaseTrailerPath + key;
            }
        }

        public void SetPoster(Movie movie)
        {

            // If the Movie objects has Images the first (poster)
            // it is added to the basic image path from tmdb.org to
            // create a URL for the poster.
            var posters = movie.Images.Posters;

            if (posters.Count > 0)
            {
                PosterUrl = "https://image.tmdb.org/t/p/w300_and_h450_bestv2/" + posters[0].FilePath;
            }
        }

        public void SetDirectors(Movie movie)
        {
            // Get the director(s) from the Movie object and add them to a
            // comma dilineated string.
            List<string> directors = new List<string>();

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
        }

        public void SetCast(Movie movie)
        {
            // Add crew from Movie object and add them to a comma dilineated
            // string.
            List<string> cast = new List<string>();


            for (int i = 0; i < movie.Credits.Cast.Count && i <= 8; i++)
            {
                cast.Add(movie.Credits.Cast[i].Name);
            }
            Cast = String.Join(", ", cast);

        }
    }
}