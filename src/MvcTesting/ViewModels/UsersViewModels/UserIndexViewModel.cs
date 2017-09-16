using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class UserIndexViewModel
    {
        public List<ApplicationUser> Users { get; set; }

        public int CurrentPage { get; set; }

        public int LastPage { get; set; }

        public UserIndexViewModel() { }
    }


}
