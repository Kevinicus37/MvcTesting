using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class DisplayUserViewModel
    {
        public IList<Film> Films { get; set; }

        public ApplicationUser User { get; set; }

        public DisplayUserViewModel() { }

        public DisplayUserViewModel(IList<Film> films, ApplicationUser user)
        {
            User = user;
            Films = films;
        }
    }
}
