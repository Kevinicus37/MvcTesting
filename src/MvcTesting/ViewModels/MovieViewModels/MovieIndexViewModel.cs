using MvcTesting.Models;
using System.Collections.Generic;
using System.Linq;

namespace MvcTesting.ViewModels
{
    public class MovieIndexViewModel
    {
        public List<Film> Films { get; set; }
        public List<string> PosterUrls { get; set; } = new List<string>();

        public MovieIndexViewModel() { }

        public MovieIndexViewModel(IList<Film> films)
        {
            Films = new List<Film>();

            for (int i =0; i < films.Count && PosterUrls.Count < 100 && Films.Count < 100; i++)
            {
                List<Film> existingFilms = Films.Where(x => x.Name == films[i].Name).ToList();

                if (!existingFilms.Any())
                {
                    if (!string.IsNullOrEmpty(films[i].PosterUrl))
                    {
                        PosterUrls.Add(films[i].PosterUrl);
                    }

                    Films.Add(new Film { ID = films[i].ID, Name = films[i].Name, Year = films[i].Year });
                }
            }
        }
    }
}
