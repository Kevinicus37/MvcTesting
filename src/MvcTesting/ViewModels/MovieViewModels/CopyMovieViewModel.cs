using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.ViewModels.MovieViewModels
{
    public class CopyMovieViewModel : EditMovieViewModel
    {
        public CopyMovieViewModel() { }

        public CopyMovieViewModel(IEnumerable<MediaFormat> mediaFormats, IEnumerable<AudioFormat> audioFormats, Film film) : base(mediaFormats, audioFormats, film)
        {
            ID = 0;

        }
    }
}
