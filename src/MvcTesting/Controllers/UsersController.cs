using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcTesting.Models;
using Microsoft.AspNetCore.Identity;
using MvcTesting.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace MvcTesting.Controllers
{
    public class UsersController : Controller
    {
        private readonly MovieCollectorContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(MovieCollectorContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(string sortBy = "UserName", bool descending = false)
        {
            List<ApplicationUser> Users;
            if (!descending)
            {
                 Users = context.Users.OrderBy(u => u.GetType().GetProperty(sortBy).GetValue(u)).ToList();
            }
            else
            {
                Users = context.Users.OrderByDescending(u => u.GetType().GetProperty(sortBy).GetValue(u)).ToList();
            }

            UserIndexViewModel userIndexViewModel = new UserIndexViewModel { Users = Users };
            return View(userIndexViewModel);
        }

        [Authorize(Roles ="Admin")]
        public IActionResult ViewRoles()
        {
            UsersViewRolesViewModel vm = new UsersViewRolesViewModel { Users = context.Users.ToList() };
            return View(vm);
        }

        public IActionResult DisplayUser(string UserName)
        {
            ApplicationUser user = context.Users.SingleOrDefault(u => u.UserName == UserName);
            List<Film> films = context.Films.Where(f => f.UserID == user.Id).ToList();
            DisplayUserViewModel displayUserViewModel = new DisplayUserViewModel(films, user);
            return View(displayUserViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(string id)
        {
            ApplicationUser user = await GetUserById(id);
            var vm = new UsersAddRoleViewModel
            {
                UserID = id,
                Roles = GetAllRoles(),
                Username = user.UserName

            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(UsersAddRoleViewModel rvm)
        {
            ApplicationUser user = await GetUserById(rvm.UserID);

            if (ModelState.IsValid)
            {
                
                var result = await _userManager.AddToRoleAsync(user, rvm.NewRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ViewRoles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                
            }
            rvm.Username = user.UserName;
            rvm.Roles = GetAllRoles();
            return View(rvm);
        }

        private async Task<ApplicationUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        private SelectList GetAllRoles()
        {
            return new SelectList(_roleManager.Roles.OrderBy(r => r.Name));
        }
    }
}