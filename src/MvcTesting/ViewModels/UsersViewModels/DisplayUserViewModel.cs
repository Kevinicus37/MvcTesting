using MvcTesting.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcTesting.ViewModels
{
    public class DisplayUserViewModel : SearchAndDisplayViewModel
    {
        [Required]
        [Display(Name ="Username")]
        public string UserName { get; set; }

        public string ProfilePicturePath { get; set; }

        public DisplayUserViewModel() { }

        public DisplayUserViewModel(IList<Film> films, string userName) : base(films)
        {
            UserName = userName;
        }
    }
}
