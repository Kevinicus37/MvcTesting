using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class GenreViewModel
    {
        public List<Genre> Items { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }

        public GenreViewModel()
        {
            Controller = "Genre";
        }
    }

    
}
