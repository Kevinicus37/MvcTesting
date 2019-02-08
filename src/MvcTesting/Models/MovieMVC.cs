using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.Movies;

namespace MvcTesting.Models
{
    public class MovieMVC
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ReleaseDate { get; set; }

        public string Directors { get; set; }

        public string PosterUrl { get; set; }

        public MovieMVC() { }

        public MovieMVC(Movie movie)
        {
            ConvertFromMovie(movie);
        }

        public void UpdatePosterPath(Movie movie)
        {
                if (movie.Images.Posters.Count > 0)
                {
                    PosterUrl = $"https://image.tmdb.org/t/p/w300_and_h450_bestv2/{movie.Images.Posters[0].FilePath}";
                }
                else
                {
                    PosterUrl = "/images/filmposterdefault.jpg";
                }
        }

        public void UpdateReleaseDate(Movie movie)
        {
            if (movie.ReleaseDate.HasValue)
            {
                ReleaseDate = "(" + movie.ReleaseDate.Value.Year.ToString() + ")";
            }
        }

        public void UpdateDirectors(Movie movie)
        {
            if (movie.Credits.Crew.Count > 0)
            {
                List<string> directors = new List<string>();
                foreach (var member in movie.Credits.Crew)
                {
                    if (member.Job == "Director")
                    {
                        directors.Add(member.Name);
                    }
                }
                Directors = String.Join(", ", directors);
            }
        }

        public void ConvertFromMovie(Movie movie)
        {
            Id = movie.Id;
            Title = movie.Title;
            UpdatePosterPath(movie);
            UpdateReleaseDate(movie);
            UpdateDirectors(movie);
        }
    }
}
