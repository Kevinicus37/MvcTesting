using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels
{
    public class AddMediaFormatViewModel
    {
        [Required]
        [Display(Name="Media Format:")]
        public string Name { get; set; }
    }
}
