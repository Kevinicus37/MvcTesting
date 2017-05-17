using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcTesting.Models;
using Microsoft.AspNetCore.Identity;
using MvcTesting.ViewModels;

namespace MvcTesting.Controllers
{
    public class UsersController : Controller
    {
        private readonly MovieCollectorContext context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(MovieCollectorContext dbContext, UserManager<ApplicationUser> userManager)
        {
            context = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            
            List<ApplicationUser> Users = context.Users.OrderBy(u => u.UserName).ToList();
            UserIndexViewModel userIndexViewModel = new UserIndexViewModel { Users = Users };
            return View(userIndexViewModel);
        }

        public IActionResult DisplayUser(string UserName)
        {
            ApplicationUser user = context.Users.SingleOrDefault(u => u.UserName == UserName);
            List<Film> films = context.Films.Where(f => f.UserID == user.Id).ToList();
            DisplayUserViewModel displayUserViewModel = new DisplayUserViewModel(films, user);
            return View(displayUserViewModel);
        }
    }
}