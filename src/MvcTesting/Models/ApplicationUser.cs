using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MvcTesting.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public DateTime Updated { get; set; }
        public IList<Film> Films { get; set; }
        public bool IsPrivate { get; set; }
        public string ProfilePicture { get; set; }
    }
}
