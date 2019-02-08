using MvcTesting.Models;
using System.Collections.Generic;
using System.Linq;

namespace MvcTesting.ViewModels
{
    public class MovieIndexViewModel
    {
        public List<Film> Films { get; set; }
        public List<Film> PosterFilms { get; set; } = new List<Film>();
        public List<ApplicationUser> Users = new List<ApplicationUser>();

        public MovieIndexViewModel() { }

        public MovieIndexViewModel(IList<Film> films)
        {
            Films = new List<Film>();

            for (int i =0; i < films.Count && PosterFilms.Count < 20 && Films.Count < 20; i++)
            {
                List<Film> existingFilms = Films.Where(x => x.Name == films[i].Name).ToList();

                if (!existingFilms.Any())
                {
                    if (!string.IsNullOrEmpty(films[i].PosterUrl) && films[i].PosterUrl != GlobalVariables.DefaultPoster)
                    {
                        PosterFilms.Add(films[i]);
                    }

                    Films.Add(new Film { ID = films[i].ID, Name = films[i].Name, Year = films[i].Year });
                }
            }
        }
    }
}
