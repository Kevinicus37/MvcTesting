using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting
{
    public static class Extensions
    {
        public static List<Film> SortByValue(this List<Film> films, string sortValue)
        {
            switch (sortValue)
            {
                case "Title":
                    return films.SortByTitle();
                case "Title Desc.":
                    return films.SortByTitleDescending();
                case "Year":
                    return films.SortByYear();
                case "Year Desc.":
                    return films.SortByYearDescending();
                case "Media Format":
                    return films.SortByMediaFormat();
                case "Media Format Desc.":
                    return films.SortByMediaFormatDescending();
                case "Audio Format":
                    return films.SortByAudioFormat();
                case "Audio Format Desc.":
                    return films.SortByAudioFormatDescending();
                default:
                    return films.SortByTitle();
            }
        }

        public static List<Film> SortByTitle(this List<Film> films)
        {
            return films.OrderBy(f => f.Name).ToList();
        }

        public static List<Film> SortByTitleDescending(this List<Film> films)
        {
            return films.OrderByDescending(f => f.Name).ToList();
        }

        public static List<Film> SortByYear(this List<Film> films)
        {
            return films.OrderBy(f => f.Year).ToList();
        }

        public static List<Film> SortByYearDescending(this List<Film> films)
        {
            return films.OrderByDescending(f => f.Year).ToList();
        }

        public static List<Film> SortByAudioFormat(this List<Film> films)
        {
            return films.OrderBy(f => f.Audio.Name).ToList();
        }

        public static List<Film> SortByAudioFormatDescending(this List<Film> films)
        {
            return films.OrderByDescending(f => f.Audio.Name).ToList();
        }

        public static List<Film> SortByMediaFormat(this List<Film> films)
        {
            return films.OrderBy(f => f.Media.Name).ToList();
        }

        public static List<Film> SortByMediaFormatDescending(this List<Film> films)
        {
            return films.OrderByDescending(f => f.Media.Name).ToList();
        }
    }
}
