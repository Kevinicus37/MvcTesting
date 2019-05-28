using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels
{
    public class SearchAndDisplayViewModel
    {
        
        public string SearchValue { get; set; }

        public string SortValue { get; set; }

        public SortPriority SortPriority { get; set; }

        public string GenreFilter { get; set; } = null;

        public string AudioFilter { get; set; } = null;

        public string MediaFilter { get; set; } = null;

        public IList<Film> Films { get; set; }

        public List<Genre> Genres { get; set; }

        public List<AudioFormat> AudioFormats { get; set; }

        public List<MediaFormat> MediaFormats { get; set; }

        public string FilterValue { get; set; }

        public SearchAndDisplayViewModel() { }

        public SearchAndDisplayViewModel(IList<Film> films)
        {
            if (films == null)
            {
                films = new List<Film>();
            }
            Films = films;
        }
    }
}
