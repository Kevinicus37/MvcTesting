using System.ComponentModel.DataAnnotations;

namespace MvcTesting.ViewModels
{
    public class AddGenreViewModel
    {
        [Required]
        [Display(Name = "Genre:")]
        public string Name { get; set; }
    }
}
