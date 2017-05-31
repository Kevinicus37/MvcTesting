using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels
{
    public class EditMediaFormatViewModel : MediaFormatViewModel
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Media Format Name:")]
        public string Name { get; set; }

        public EditMediaFormatViewModel()
        {
            Action = "Edit";
        }
    }
}
