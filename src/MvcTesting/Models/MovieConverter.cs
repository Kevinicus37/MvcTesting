using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.Movies;

namespace MvcTesting.Models
{
    public class MovieConverter
    {
        public Film ConvertToFilm(Movie movie, List<Genre> genres)
        {
            Film convertedFilm = new Film();

            convertedFilm.Name = movie.Title;
            convertedFilm.Overview = movie.Overview;
            convertedFilm.Runtime = movie.Runtime;
            convertedFilm.PosterUrl = GetConvertedPosterPath(movie);
            convertedFilm.TrailerUrl = GetConvertedTrailerPath(movie);
            convertedFilm.Year = movie.ReleaseDate.Value.Year;
            convertedFilm.Cast = GetConvertedCastMembers(movie);
            convertedFilm.Directors = GetConvertedDirectors(movie);

            List<Genre> filmGenres = GetMatchingGenres(movie, genres);
            foreach (Genre genre in filmGenres)
            {
                convertedFilm.FilmGenres.Add(new FilmGenre { Genre = genre, GenreID = genre.ID });
            }

            return convertedFilm;
        }

        public string GetConvertedPosterPath(Movie movie)
        {
            string posterUrl = GlobalVariables.DefaultPoster;

            if (movie.Images.Posters.Count > 0)
            {
                posterUrl = $"{GlobalVariables.TMDBPosterSitePath}{movie.Images.Posters[0].FilePath}";
            }
            
            return posterUrl;
        }

        public string GetConvertedTrailerPath(Movie movie)
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
        
        public string GetConvertedReleaseDate(Movie movie)
        {
            string releaseDate = "";

            if (movie.ReleaseDate.HasValue)
            {
                releaseDate = "(" + movie.ReleaseDate.Value.Year.ToString() + ")";
            }

            return releaseDate;
        }

        public string GetConvertedDirectors(Movie movie)
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

        public string GetConvertedCastMembers(Movie movie)
        {
            List<string> convertedCastMembers = new List<string>();

            for (int i = 0; i < movie.Credits.Cast.Count && i <= 8; i++)
            {
                convertedCastMembers.Add(movie.Credits.Cast[i].Name);
            }

            return String.Join(",", convertedCastMembers);
        }

        public List<string> GetConvertedCastMembersList(Movie movie)
        {
            List<string> convertedCastMembers = new List<string>();

            for (int i = 0; i < movie.Credits.Cast.Count && i <= 8; i++)
            {
                convertedCastMembers.Add(movie.Credits.Cast[i].Name);
            }

            return convertedCastMembers;

        }

        public List<Genre> GetMatchingGenres(Movie movie, List<Genre> genres)
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
