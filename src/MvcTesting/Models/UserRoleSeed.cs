using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MvcTesting.Models
{
    public class UserRoleSeed
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoleSeed(RoleManager<IdentityRole> rolemanager)
        {
            _roleManager = rolemanager;
        }

        public async void Seed()
        {
            if (await _roleManager.FindByNameAsync("Member") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });
            }

            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            }
        }
    }
}
