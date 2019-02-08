using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting
{
    public static class GlobalVariables
    {
        public static string DefaultProfilePicture { get; } = "/images/profilePictureDefault.png";

        public static string DefaultPoster { get; } = "/images/filmposterdefault.jpg";

        public static string TMDBPosterSitePath { get; } = "https://image.tmdb.org/t/p/w300_and_h450_bestv2/";

        public static string BaseTrailerPath { get; } = "https://www.youtube.com/embed/";
    }
}
