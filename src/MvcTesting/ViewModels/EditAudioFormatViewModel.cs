using System.ComponentModel.DataAnnotations;

namespace MvcTesting.ViewModels
{
    public class EditAudioFormatViewModel : AudioFormatViewModel
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Genre Name:")]
        public string Name { get; set; }

        public EditAudioFormatViewModel()
        {
            Action = "Edit";
        }
    }
}
