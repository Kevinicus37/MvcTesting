using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class DisplayUserViewModel
    {
        public IList<Film> Films { get; set; }

        public ApplicationUser User { get; set; }

        public List<Genre> Genres { get; set; }

        public string FilterValue { get; set; }

        public List<MediaFormat> MediaFormats { get; set; }

        public DisplayUserViewModel() { }

        public DisplayUserViewModel(IList<Film> films, ApplicationUser user)
        {
            User = user;
            Films = films;
        }
    }
}
