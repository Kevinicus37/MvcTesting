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

        public string TrailerUrl { get; set; }

        public string Overview { get; set; }

        public List<string> CastMembers { get; set; } = new List<string>();

        public int? RunTime { get; set; }

        public List<string> Genres { get; set; } = new List<string>();

        public MovieMVC() { }

        public MovieMVC(Movie movie)
        {
            ConvertFromMovie(movie);
        }

        public MovieMVC(Movie movie, List<string> genres) : this(movie)
        {
            UpdateGenres(movie, genres);
        }

        public void UpdatePosterPath(Movie movie)
        {
                if (movie.Images.Posters.Count > 0)
                {
                    PosterUrl = $"{GlobalVariables.TMDBPosterSitePath}{movie.Images.Posters[0].FilePath}";
                }
                else
                {
                    PosterUrl = GlobalVariables.DefaultPoster;
                }
        }

        public void UpdateTrailerPath(Movie movie)
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

        public void UpdateCastMembers(Movie movie)
        {
            for (int i = 0; i < movie.Credits.Cast.Count && i <= 8; i++)
            {
                CastMembers.Add(movie.Credits.Cast[i].Name);
            }
        }

        public void ConvertFromMovie(Movie movie)
        {
            Id = movie.Id;
            Title = movie.Title;
            UpdatePosterPath(movie);
            UpdateReleaseDate(movie);
            UpdateDirectors(movie);
            UpdateTrailerPath(movie);
            RunTime = movie.Runtime;
            Overview = movie.Overview;
            UpdateCastMembers(movie);
        }

        public void UpdateGenres(Movie movie, List<string> genres)
        {
            foreach (var genre in movie.Genres)
            {
                if (genres.Contains(genre.Name))
                {
                    Genres.Add(genre.Name);
                }

                if (genre.Name == "Science Fiction")
                {
                    Genres.Add("Sci-Fi");
                }
            }
        }
    }

   
}
