using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels
{
    public class RemoveGenreViewModel : GenreViewModel
    {
        
        public RemoveGenreViewModel()
        {
            Action = "RemoveGenre";
        }
    }
}
