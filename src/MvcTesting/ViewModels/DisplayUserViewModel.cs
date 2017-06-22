using MvcTesting.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcTesting.ViewModels
{
    public class DisplayUserViewModel
    {
        [Required]
        [Display(Name ="Username")]
        public string UserName { get; set; }

        public string SearchValue { get; set; }

        public string SortValue { get; set; }

        public List<string> SortValues = new List<string> { "Title", "Title Desc.", "Year", "Year Desc.", "Audio Format", "Audio Format Desc.", "Media Format", "Media Format Desc." };

        public string PropertyType { get; set; }

        public IList<Film> Films { get; set; }

        //public ApplicationUser User { get; set; }

        public List<Genre> Genres { get; set; }

        public List<AudioFormat> AudioFormats { get; set; }

        public List<MediaFormat> MediaFormats { get; set; }

        public string FilterValue { get; set; }

        public DisplayUserViewModel() { }

        public DisplayUserViewModel(IList<Film> films, string userName)
        {
            //User = user;
            UserName = userName;
            if (films == null)
            {
                films = new List<Film>();
            }
            Films = films;
        }
    }
}
