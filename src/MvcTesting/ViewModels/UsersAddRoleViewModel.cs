using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcTesting.ViewModels
{
    public class UsersAddRoleViewModel
    {

        public string Username { get; set; }

        public string NewRole { get; set; }

        public SelectList Roles { get; set; }

        public string UserID { get; set; }
    }
}
