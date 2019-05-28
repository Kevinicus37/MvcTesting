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
using MvcTesting.Services;

namespace MvcTesting.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly MovieCollectorContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserServices _userServices;
        private readonly FilmServices _filmServices;

        public UsersController(MovieCollectorContext dbContext, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            UserServices userServices,
            FilmServices filmServices)
        {
            _context = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _userServices = userServices;
            _filmServices = filmServices;
        }

        [AllowAnonymous]
        public IActionResult Index(string sortBy = "UserName", bool descending = false, int page = 1)
        {

            // Display a list of users according to the value of sortBy in either ascending
            // or descending order based on the value of the descending variable.
            int perPage = 20;
            int skip = (page - 1) * perPage;
            List<ApplicationUser> users = _userServices.GetAllPublicUsers(User);
            int lastPage = users.Count / perPage;
            if (users.Count % perPage > 0) { lastPage++; }
            users = users.Skip(skip).Take(perPage).ToList();

            if (!descending)
            {
                 users = users.OrderBy(u => u.GetType().GetProperty(sortBy).GetValue(u)).ToList();

            }
            else
            {
                users = users.OrderByDescending(u => u.GetType().GetProperty(sortBy).GetValue(u)).ToList();
            }

            UserIndexViewModel userIndexViewModel = new UserIndexViewModel { Users = users, CurrentPage = page, LastPage = lastPage };
            return View(userIndexViewModel);
        }

        public IActionResult ViewRoles()
        {
            UsersViewRolesViewModel vm = new UsersViewRolesViewModel { Users = _context.Users.ToList() };
            return View(vm);
        }

        public IActionResult Delete(string userName)
        {
            
            return View(new UsersDeleteViewModel { UserName = userName });
        
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UsersDeleteViewModel vm)
        {
            var user = await _userManager.FindByNameAsync(vm.UserName);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult DisplayUser(string UserName)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.UserName == UserName);
            List<Film> films = new List<Film>();
            
            // If someone attempts to access a page for a private User directly and is not that User
            // and is not an Admin, they are redirected to the Index Action.
            if (user == null || (user.IsPrivate && !User.IsInRole("Admin") && user.Id != _userManager.GetUserId(User)))
            {
                return RedirectToAction("Index");
            }

            films = _userServices.GetUserFilms(user, User).ToList();
            DisplayUserViewModel vm = new DisplayUserViewModel(films, user.UserName);
            vm.Genres = _context.Genres.ToList();
            vm.MediaFormats = _context.MediaFormats.ToList();
            vm.AudioFormats = _context.AudioFormats.ToList();

            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                vm.ProfilePicturePath = GlobalVariables.ImagesBasePath + user.UserName + "/" + user.ProfilePicture;
            }
            else
            {
                vm.ProfilePicturePath = GlobalVariables.DefaultProfilePicture;
            }
            
            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult DisplayUser(DisplayUserViewModel vm)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.UserName == vm.UserName);
            
            // If someone attempts to access a page for a private User directly and is not that User
            // and is not an Admin, they are redirected to the Index Action.
            if (!ModelState.IsValid || (user.IsPrivate && !User.IsInRole("Admin") && user.Id != _userManager.GetUserId(User)))
            {
                return RedirectToAction("Index");
            }

            List<Film> films = new List<Film>();

            films = _userServices.GetUserFilms(user, User).Include(f => f.FilmGenres).ThenInclude(fg => fg.Genre).ToList();
            films = films
                .Where(f => string.IsNullOrEmpty(vm.AudioFilter) || f.Audio.Name == vm.AudioFilter)
                .Where(f => string.IsNullOrEmpty(vm.MediaFilter) || f.Media.Name == vm.MediaFilter)
                .Where(f => string.IsNullOrEmpty(vm.GenreFilter) || f.FilmGenres.Any(fg => fg.Genre.Name == vm.GenreFilter))
                .Where(f => string.IsNullOrEmpty(vm.SearchValue) || f.Name.ToLower().Contains(vm.SearchValue.ToLower()))
                .ToList();

            vm.Films = _filmServices.SortBy(films, vm.SortPriority);
            vm.Genres = _context.Genres.ToList();
            vm.MediaFormats = _context.MediaFormats.ToList();
            vm.AudioFormats = _context.AudioFormats.ToList();

            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                vm.ProfilePicturePath = GlobalVariables.ImagesBasePath + user.UserName + "/" + user.ProfilePicture;
            }
            else
            {
                vm.ProfilePicturePath = GlobalVariables.DefaultProfilePicture;
            }

            return View(vm);
        }
 
        [HttpGet]
        public async Task<IActionResult> AddRole(string id)
        {
            // Find the User by id and seed the ViewModel
            ApplicationUser user = await _userServices.GetUserByIdAsync(id);
            if (user != null)
            {
                var vm = new UsersAddRoleViewModel
                {
                    UserID = id,
                    Roles = _userServices.GetAllRoles(),
                    Username = user.UserName

                };

                return View(vm);
            }
            return RedirectToAction("ViewRoles");
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(UsersAddRoleViewModel vm)
        {
            // Allows an Admin to add a Role to another User.  

            // Get the user to add a Role to from the ViewModel
            ApplicationUser user = await _userServices.GetUserByIdAsync(vm.UserID);

            // If VM is valid, try to add user to Role.  If successful Redirect to
            // the ViewRoles action.  Otherwise, return to the view with the VM and
            // errors.
            if (ModelState.IsValid)
            {
                
                var result = await _userManager.AddToRoleAsync(user, vm.NewRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ViewRoles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                
            }
            vm.Username = user.UserName;
            vm.Roles = _userServices.GetAllRoles();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveRole(string id)
        {
            // Find the User by id and seed the ViewModel
            ApplicationUser user = await _userServices.GetUserByIdAsync(id);
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
        public async Task<IActionResult> RemoveRole(UsersRemoveRoleViewModel vm)
        {
            // Allows an Admin to Remove a Role from another User.  

            // Get the user to add a Role to from the ViewModel
            ApplicationUser user = await _userServices.GetUserByIdAsync(vm.UserID);

            // If VM is valid, try to remove Role from user.  If successful Redirect to
            // the ViewRoles Action.  Otherwise, return to the view with the VM and
            // errors.
            if (ModelState.IsValid)
            {
                foreach (string role in vm.Roles)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, role);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                        vm.Username = user.UserName;
                        vm.Roles = await _userManager.GetRolesAsync(user);
                        return View(vm);
                    }
                }

                return RedirectToAction("ViewRoles");
            }
            
            vm.Username = user.UserName;
            vm.Roles = await _userManager.GetRolesAsync(user);
            return View(vm);
                
                    
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Search(UsersSearchViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.UserQuery))
            {
                return RedirectToAction("Index");
            }
            List<ApplicationUser> Users = _userServices.GetAllPublicUsers(User).Where(u => u.UserName.ToLower().Contains(vm.UserQuery.ToLower())).OrderBy(u=>u.UserName).ToList();
            vm.Users = Users;
            return View(vm);
        }
    }
}