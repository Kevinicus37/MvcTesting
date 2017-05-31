using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class UserIndexViewModel
    {
        public List<ApplicationUser> Users { get; set; }

        public UserIndexViewModel() { }
    }


}
