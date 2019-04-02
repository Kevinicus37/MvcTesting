using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcTesting.Services
{
    public class FilmServices
    {
        private readonly MovieCollectorContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FilmServices(MovieCollectorContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<Film> GetAllPublicFilmsWithUser(ClaimsPrincipal appUser)
        {
            return _context.Films.Include(f => f.User).OrderBy(f => f.Name)
                .Where(f => (!f.IsPrivate && !f.User.IsPrivate) || f.UserID == _userManager.GetUserId(appUser) || appUser.IsInRole("Admin"))
                .OrderByDescending(f => f.Updated)
                .ToList();
        }

        public List<Film> GetAllPublicCompleteFilms(ClaimsPrincipal appUser)
        {
            return _context.Films.Include(f => f.User)
                .Include(f => f.Media)
                .Include(f => f.Audio)
                .Include(f => f.FilmGenres)
                    .ThenInclude(fg => fg.Genre)
                .OrderBy(f => f.Name)
                .Where(f => (!f.IsPrivate && !f.User.IsPrivate) || f.UserID == _userManager.GetUserId(appUser) || appUser.IsInRole("Admin"))
                .OrderByDescending(f => f.Updated)
                .ToList();
        }

        public List<Film> GetAllCompleteFilms()
        {
            return _context.Films.Include(f => f.User)
                .Include(f => f.Media)
                .Include(f => f.Audio)
                .Include(f => f.FilmGenres)
                    .ThenInclude(fg => fg.Genre)
                .OrderBy(f => f.Name)
                .OrderByDescending(f => f.Updated)
                .ToList();
        }
    }
}
