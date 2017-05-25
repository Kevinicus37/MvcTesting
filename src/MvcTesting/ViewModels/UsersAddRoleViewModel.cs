using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MvcTesting.ViewModels
{
    public class UsersAddRoleViewModel
    {

        public string Username { get; set; }

        [Required]
        public string NewRole { get; set; }

        public SelectList Roles { get; set; }

        public string UserID { get; set; }
    }
}
