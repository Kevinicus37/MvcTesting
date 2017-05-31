using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels
{
    public class UsersRemoveRoleViewModel
    {
        public string Username { get; set; }
        public string UserID { get; set; }
        public IList<string> Roles { get; set; }
    }
}
