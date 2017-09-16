using System.ComponentModel.DataAnnotations;

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
