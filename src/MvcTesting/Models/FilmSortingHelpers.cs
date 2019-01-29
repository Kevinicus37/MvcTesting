using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.Models
{
    public static class FilmSortingHelpers
    {

        public static List<Film> SortByValue(this List<Film> films, string sortValue)
        {
            switch (sortValue)
            {
                case "Title":
                    return SortByTitle(films);
                case "Title Desc.":
                    return SortByTitleDescending(films);
                case "Year":
                    return SortByYear(films);
                case "Year Desc.":
                    return SortByYearDescending(films);
                case "Media Format":
                    return SortByMediaFormat(films);
                case "Media Format Desc.":
                    return SortByMediaFormatDescending(films);
                case "Audio Format":
                    return SortByAudioFormat(films);
                case "Audio Format Desc.":
                    return SortByAudioFormatDescending(films);
                default:
                    return SortByTitle(films);
            }
        }

        public static List<Film> SortByTitle(List<Film> films)
        {
            return films.OrderBy(f => f.Name).ToList();
        }

        public static List<Film> SortByTitleDescending(List<Film> films)
        {
            return films.OrderByDescending(f => f.Name).ToList();
        }

        public static List<Film> SortByYear(List<Film> films)
        {
            return films.OrderBy(f => f.Year).ToList();
        }

        public static List<Film> SortByYearDescending(List<Film> films)
        {
            return films.OrderByDescending(f => f.Year).ToList();
        }

        public static List<Film> SortByAudioFormat(List<Film> films)
        {
            return films.OrderBy(f => f.Audio.Name).ToList();
        }

        public static List<Film> SortByAudioFormatDescending(List<Film> films)
        {
            return films.OrderByDescending(f => f.Audio.Name).ToList();
        }

        public static List<Film> SortByMediaFormat(List<Film> films)
        {
            return films.OrderBy(f => f.Media.Name).ToList();
        }

        public static List<Film> SortByMediaFormatDescending(List<Film> films)
        {
            return films.OrderByDescending(f => f.Media.Name).ToList();
        }
    }
}
