using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels
{
    public class UserIndexViewModel
    {
        public List<ApplicationUser> Users { get; set; }

        public UserIndexViewModel() { }
    }


}
