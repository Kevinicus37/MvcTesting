using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcTesting.Services
{
    public class UserServices
    {
        private readonly MovieCollectorContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserServices(MovieCollectorContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IQueryable<Film> GetUserFilms(ApplicationUser user, ClaimsPrincipal appUser)
        {
            // Returns all Films of the user that the current User is authorized to see.  Audio, Media, and User of the film is included.
                return _context.Films
                    .Where(f => 
                        (f.UserID == user.Id) 
                        && (!f.IsPrivate || appUser.IsInRole("Admin") || f.UserID == _userManager.GetUserId(appUser))
                        )
                    .Include(f => f.Audio)
                    .Include(f => f.Media)
                    .Include(f => f.User);
        }

        public List<ApplicationUser> GetAllPublicUsers(ClaimsPrincipal appUser)
        {
            // Returns all users that the current User is authorized to see
            return _context.Users.Where(u => !u.IsPrivate || appUser.IsInRole("Admin") || u.Id == _userManager.GetUserId(appUser)).ToList();
        }

        public List<ApplicationUser> TakeUsers(int count)
        {
            return _context.Users.Include(u => u.Films).Where(u => u.IsPrivate == false)
                .Where(u => u.Films.Count > 0)
                .OrderByDescending(u => u.Films.Count)
                .Take(count)
                .ToList();
        }

        public SelectList GetAllRoles()
        {
            //Returns all available Roles as a SelectList ordered by Name
            return new SelectList(_roleManager.Roles.OrderBy(r => r.Name));
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            //Returns a User by the id.
            return await _userManager.FindByIdAsync(id);
        }
    }
}
