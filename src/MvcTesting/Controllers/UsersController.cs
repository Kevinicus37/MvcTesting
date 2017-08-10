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
            List<ApplicationUser> Users = GetUsers();
            if (!descending)
            {
                 Users = Users.OrderBy(u => u.GetType().GetProperty(sortBy).GetValue(u)).ToList();

            }
            else
            {
                Users = Users.OrderByDescending(u => u.GetType().GetProperty(sortBy).GetValue(u)).ToList();
            }

            UserIndexViewModel userIndexViewModel = new UserIndexViewModel { Users = Users };
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

            films = GetUserFilms(user).ToList();
            DisplayUserViewModel vm = new DisplayUserViewModel(films, user.UserName);
            vm.Genres = _context.Genres.ToList();
            vm.MediaFormats = _context.MediaFormats.ToList();
            vm.AudioFormats = _context.AudioFormats.ToList();

            
            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult DisplayUser(DisplayUserViewModel vm)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.UserName == vm.UserName);
            List<Film> films = new List<Film>();

            // If someone attempts to access a page for a private User directly and is not that User
            // and is not an Admin, they are redirected to the Index Action.
            if (!ModelState.IsValid || (user.IsPrivate && !User.IsInRole("Admin") && user.Id != _userManager.GetUserId(User)))
            {
                return RedirectToAction("Index");
            }

                switch (vm.PropertyType)
                {
                    case "Genre":
                            vm.FilterValue = (!_context.Genres.Any(g => g.Name == vm.FilterValue) ? null : vm.FilterValue);
                            films = GetUserFilmsByGenre(user, vm.FilterValue);
                            break;
                    case "MediaFormat":
                            vm.FilterValue = (!_context.MediaFormats.Any(mf => mf.Name == vm.FilterValue) ? null : vm.FilterValue);
                            films = GetUserFilmsByMediaFormat(user, vm.FilterValue);
                            break;
                    case "AudioFormat":
                            vm.FilterValue = (!_context.AudioFormats.Any(af => af.Name == vm.FilterValue) ? null : vm.FilterValue);
                            films = GetUserFilmsByAudioFormat(user, vm.FilterValue);
                            break;
                    case "Film":
                            if (string.IsNullOrEmpty(vm.SearchValue))
                            {
                                return Redirect("DisplayUser/?UserName=" + vm.UserName);
                            }

                            string title = (!_context.Films.Any(f => f.Name.ToLower().Contains(vm.SearchValue.ToLower())) ? null : vm.SearchValue);

                            if (!string.IsNullOrEmpty(title))
                            {
                                films = SearchUserFilmsByTitle(user, title);
                            }

                            break;
                    default:
                            films = GetUserFilms(user).ToList();
                            break;

                }
                
                vm.Films = FilmSortingHelpers.SortByValue(films, vm.SortValue);
                vm.Genres = _context.Genres.ToList();
                vm.MediaFormats = _context.MediaFormats.ToList();
                vm.AudioFormats = _context.AudioFormats.ToList();

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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Search(UsersSearchViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.UserQuery))
            {
                return RedirectToAction("Index");
            }
            List<ApplicationUser> Users = GetUsers().Where(u => u.UserName.ToLower().Contains(vm.UserQuery.ToLower())).OrderBy(u=>u.UserName).ToList();
            vm.Users = Users;
            return View(vm);
        }


        // Helper Functions

        [NonAction]
        private async Task<ApplicationUser> GetUserById(string id)
        {
            //Returns a User by the id.
            return await _userManager.FindByIdAsync(id);
        }

        [NonAction]
        private SelectList GetAllRoles()
        {
            //Returns all available Roles as a SelectList ordered by Name
            return new SelectList(_roleManager.Roles.OrderBy(r => r.Name));
        }

        [NonAction]
        private IQueryable<Film> GetUserFilms(ApplicationUser user)
        {
            // Returns all Films of the user that the current User is authorized to see.  Audio, Media, and User of the film is included.
            return _context.Films
                .Where(f => (f.UserID == user.Id) && (!f.IsPrivate || User.IsInRole("Admin") || f.UserID == _userManager.GetUserId(User)))
                .Include(f => f.Audio)
                .Include(f => f.Media)
                .Include(f => f.User);
        }

        [NonAction]
        private List<Film> GetUserFilmsByGenre(ApplicationUser user, string genre = null)
        {
            // Returns all Films of the user for a specific genre, displaying only those that the current User is authorized to see 
            return GetUserFilms(user).Include(f => f.FilmGenres).Where(f => f.FilmGenres.Any(fg => fg.Genre.Name == genre)).ToList();
        }

        [NonAction]
        private List<Film> GetUserFilmsByMediaFormat(ApplicationUser user, string mediaFormat = null)
        {
            // Returns all Films of the user for a specific MediaFormat, displaying only those that the current User is authorized to see 

            return GetUserFilms(user).Where(f => f.Media.Name == mediaFormat).ToList();
        }

        [NonAction]
        private List<Film> GetUserFilmsByAudioFormat(ApplicationUser user, string audioFormat = null)
        {
            // Returns all Films of the user for a specific AudioFormat, displaying only those that the current User is authorized to see 

            return GetUserFilms(user).Where(f => f.Audio.Name == audioFormat).ToList();
        }

        [NonAction]
        private List<ApplicationUser> GetUsers()
        {
            // Returns all users that the current User is authorized to see
            return _context.Users.Where(u => !u.IsPrivate || User.IsInRole("Admin") || u.Id == _userManager.GetUserId(User)).ToList();
        }

        [NonAction]
        private List<Film> SearchUserFilmsByTitle(ApplicationUser user, string title = null)
        {
            // Returns all Films of the user containing the search parameter (title), displaying only those that the current User is authorized to see 

            if (string.IsNullOrEmpty(title))
            {
                return new List<Film>();
            }
            return GetUserFilms(user).Where(f => f.Name.ToLower().Contains(title.ToLower())).ToList();
        }

        

    }
}