using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels
{
    public class MovieIndexViewModel
    {
        public List<Film> Films { get; set; }
        public List<string> PosterUrls { get; set; }

        public MovieIndexViewModel() { }

        public MovieIndexViewModel(IList<Film> films)
        {
            PosterUrls = new List<string>();
            Films = new List<Film>();

            for (int i =0; i < films.Count && PosterUrls.Count < 10 && Films.Count < 10; i++)
            {
                if (!string.IsNullOrEmpty(films[i].PosterUrl))
                {
                    PosterUrls.Add(films[i].PosterUrl);
                }

                Films.Add(films[i]);
            }
        }
    }
}
