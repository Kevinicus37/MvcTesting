using System.ComponentModel.DataAnnotations;

namespace MvcTesting.ViewModels
{
    public class AddAudioFormatViewModel
    {
        [Required]
        [Display(Name="Audio Format:")]
        public string Name { get; set; }


    }
}
