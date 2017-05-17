using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
