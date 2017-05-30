using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels
{
    public class EditGenreViewModel : GenreViewModel
    {
        public int ID { get; set; }

        [Required]
        [Display(Name="Genre Name:")]
        public string Name { get; set; }

        public EditGenreViewModel()
        {
            Action = "Edit";
        }
    }

    
}
