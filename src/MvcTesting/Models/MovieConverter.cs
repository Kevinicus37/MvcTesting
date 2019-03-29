using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.Movies;

namespace MvcTesting.Models
{
    public static class MovieConverter
    {
        public static Film ConvertToFilm(this Movie movie, List<Genre> genres)
        {
            Film convertedFilm = new Film();

            convertedFilm.Name = movie.Title;
            convertedFilm.Overview = movie.Overview;
            convertedFilm.Runtime = movie.Runtime;
            convertedFilm.PosterUrl = movie.GetConvertedPosterPath();
            convertedFilm.TrailerUrl = movie.GetConvertedTrailerPath();
            convertedFilm.Year = movie.ReleaseDate.Value.Year;
            convertedFilm.Cast = movie.GetConvertedCastMembers();
            convertedFilm.Directors = movie.GetConvertedDirectors();

            List<Genre> filmGenres = movie.GetMatchingGenres(genres);
            foreach (Genre genre in filmGenres)
            {
                convertedFilm.FilmGenres.Add(new FilmGenre { Genre = genre, GenreID = genre.ID });
            }

            return convertedFilm;
        }

        public static string GetConvertedPosterPath(this Movie movie)
        {
            string posterUrl = GlobalVariables.DefaultPoster;

            if (movie.Images.Posters.Count > 0)
            {
                posterUrl = $"{GlobalVariables.TMDBPosterSitePath}{movie.Images.Posters[0].FilePath}";
            }
            
            return posterUrl;
        }

        public static string GetConvertedTrailerPath(this Movie movie)
        {
            string trailerUrl = "";

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

                trailerUrl = GlobalVariables.BaseTrailerPath + key;
            }

            return trailerUrl;
        }


        public static string GetConvertedReleaseDate(this Movie movie)
        {
            string releaseDate = "";

            if (movie.ReleaseDate.HasValue)
            {
                releaseDate = "(" + movie.ReleaseDate.Value.Year.ToString() + ")";
            }

            return releaseDate;
        }

        public static string GetConvertedDirectors(this Movie movie)
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
                return String.Join(", ", directors);
            }
            return "";
        }

        public static string GetConvertedCastMembers(this Movie movie)
        {
            List<string> convertedCastMembers = new List<string>();

            for (int i = 0; i < movie.Credits.Cast.Count && i <= 8; i++)
            {
                convertedCastMembers.Add(movie.Credits.Cast[i].Name);
            }

            return String.Join(",", convertedCastMembers);
        }

        public static List<string> GetConvertedCastMembersList(this Movie movie)
        {
            List<string> convertedCastMembers = new List<string>();

            for (int i = 0; i < movie.Credits.Cast.Count && i <= 8; i++)
            {
                convertedCastMembers.Add(movie.Credits.Cast[i].Name);
            }

            return convertedCastMembers;

        }

        public static List<Genre> GetMatchingGenres(this Movie movie, List<Genre> genres)
        {
            List<Genre> movieGenres = new List<Genre>();
            
            foreach (var genre in movie.Genres)
            {
                Genre newGenre = genres.Where(x => x.Name == genre.Name).FirstOrDefault();
                if (newGenre != null)
                {
                    movieGenres.Add(newGenre);
                }
            }

            return movieGenres;
            
        }
    }
}
