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
using Microsoft.EntityFrameworkCore;

namespace MvcTesting.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly MovieCollectorContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(MovieCollectorContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public IActionResult Index(string sortBy = "UserName", bool descending = false)
        {

            // Display a list of users according to the value of sortBy in either ascending
            // or descending order based on the value of the descending variable.
            List<ApplicationUser> Users;
            if (!descending)
            {
                 Users = _context
                    .Users
                    .Where(u => !u.IsPrivate || User.IsInRole("Admin") || u.Id ==_userManager.GetUserId(User))
                    .OrderBy(u => u.GetType().GetProperty(sortBy).GetValue(u)).ToList();
            }
            else
            {
                Users = _context.Users.OrderByDescending(u => u.GetType().GetProperty(sortBy).GetValue(u)).ToList();
            }

            UserIndexViewModel userIndexViewModel = new UserIndexViewModel { Users = Users };
            return View(userIndexViewModel);
        }



        public IActionResult ViewRoles()
        {
            UsersViewRolesViewModel vm = new UsersViewRolesViewModel { Users = _context.Users.ToList() };
            return View(vm);
        }


        [AllowAnonymous]
        public IActionResult DisplayUser(string UserName, string propertyType = null, string propertyValue = null)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.UserName == UserName);
            List<Film> films;
            DisplayUserViewModel vm;

            if (user == null)
            {
                return RedirectToAction("Index");
            }
            // If someone attempts to access a page for a private User directly and is not that User
            // and is not an Admin, they are redirected to the Index Action.
            if (user.IsPrivate && !User.IsInRole("Admin") && user.Id != _userManager.GetUserId(User))
            {
                return RedirectToAction("Index");
            }

            switch (propertyType)
            {
                case "Genre": films = GetUserFilmsByGenre(user, propertyValue).ToList();
                    break;
                default: films = GetUserFilms(user).ToList();
                    break;

            }
            vm = new DisplayUserViewModel(films, user);
            vm.Genres = _context.Genres.ToList();

            
            return View(vm);
        }

        

        [HttpGet]
        public async Task<IActionResult> AddRole(string id)
        {
            // Find the User by id and seed the ViewModel
            ApplicationUser user = await GetUserById(id);
            if (user != null)
            {
                var vm = new UsersAddRoleViewModel
                {
                    UserID = id,
                    Roles = GetAllRoles(),
                    Username = user.UserName

                };

                return View(vm);

            }
            return RedirectToAction("ViewRoles");
        }



        [HttpPost]
        public async Task<IActionResult> AddRole(UsersAddRoleViewModel rvm)
        {
            // Allows an Admin to add a Role to another User.  

            // Get the user to add a Role to from the ViewModel
            ApplicationUser user = await GetUserById(rvm.UserID);

            // If VM is valid, try to add user to Role.  If successful Redirect to
            // the ViewRoles action.  Otherwise, return to the view with the VM and
            // errors.
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


        [HttpGet]
        public async Task<IActionResult> RemoveRole(string id)
        {
            // Find the User by id and seed the ViewModel
            ApplicationUser user = await GetUserById(id);
            if (user != null)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);

                var vm = new UsersRemoveRoleViewModel
                {
                    Username = user.UserName,
                    UserID = user.Id,
                    Roles = roles
                };
                

                return View(vm);

            }
            return RedirectToAction("ViewRoles");
        }



        [HttpPost]
        public async Task<IActionResult> RemoveRole(UsersRemoveRoleViewModel rvm)
        {
            // Allows an Admin to Remove a Role from another User.  

            // Get the user to add a Role to from the ViewModel
            ApplicationUser user = await GetUserById(rvm.UserID);

            // If VM is valid, try to remove Role from user.  If successful Redirect to
            // the ViewRoles Action.  Otherwise, return to the view with the VM and
            // errors.
            if (ModelState.IsValid)
            {
                foreach (string role in rvm.Roles)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, role);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                        rvm.Username = user.UserName;
                        rvm.Roles = await _userManager.GetRolesAsync(user);
                        return View(rvm);
                    }
                }

                return RedirectToAction("ViewRoles");
            }
            
            rvm.Username = user.UserName;
            rvm.Roles = await _userManager.GetRolesAsync(user);
            return View(rvm);
                
                    
        }


        private async Task<ApplicationUser> GetUserById(string id)
        {
            //Returns a User by the id.
            return await _userManager.FindByIdAsync(id);
        }



        private SelectList GetAllRoles()
        {
            //Returns all available Roles as a SelectList ordered by Name
            return new SelectList(_roleManager.Roles.OrderBy(r => r.Name));
        }


        private IQueryable<Film> GetUserFilms(ApplicationUser user)
        {
            return _context.Films
                .Include(f => f.User)
                .Where(f => (f.UserID == user.Id) && (!f.IsPrivate || User.IsInRole("Admin") || f.UserID == _userManager.GetUserId(User)));
        }

        private IQueryable<Film> GetUserFilmsByGenre(ApplicationUser user, string genre = null)
        {
            IQueryable<Film> films = GetUserFilms(user);

            if (!string.IsNullOrEmpty(genre))
            {
                films = films.Include(f => f.FilmGenres).Where(f => f.FilmGenres.Any(fg => fg.Genre.Name == genre));
            }
            return films;
        }
    }
}