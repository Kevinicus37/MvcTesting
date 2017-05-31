using System.ComponentModel.DataAnnotations;

namespace MvcTesting.ViewModels
{
    public class AddMediaFormatViewModel
    {
        [Required]
        [Display(Name="Media Format:")]
        public string Name { get; set; }
    }
}
