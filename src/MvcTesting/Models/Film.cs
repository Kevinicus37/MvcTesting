using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.Models
{
    public class Film
    {
        // ID for database
        public int ID { get; set; }  

        // Title of Movie/Disc
        public string Name { get; set; }

        // Year of release (theatrical)
        public int Year { get; set; }

        // Format (DVD, Blu-ray, 4K UHD, Digital, etc.) 
        // This is a one-to many relationship
        public MediaFormat Media { get; set; }
        public int MediaID { get; set; }

        // Aspect ratio of film (1.33:1, 1.77:1, 1.85:1, 16:9, 2.35:1, 2.39:1, 2.4:1, shifting, etc.)
        public string AspectRatio { get; set; }

        // Film Id on TMDb.org
        public int TMDbId { get; set; }

        // Thoughts a user has on the movie
        public string Comments { get; set; }

        // Rating (1-10) that the user gives the movie.
        public int Rating { get; set; }

        // Director(s) of the movie
        // Will have to be split/joined when displayed
        public string Directors { get; set; }

        // Cast Members
        // Will have to be split/joined when displayed
        public string Cast { get; set; }

        // Audio Format (Dolby Digital, DTS, Dolby True HD, DTS-MA HD, Dolby Atmos, DTS-X, Auro, Stereo, Mono, etc.)
        public AudioFormat Audio{ get; set; }
        public int AudioID { get; set; }

        // A general description of what the movie is about.
        public string Overview { get; set; }

        // Genres the film belongs to - FilmGenre is a database for a many-to-many relationship.
        public IList<FilmGenre> Genres { get; set; }

        // Url for the film's trailer on Youtube.com (or elsewhere)
        public string TrailerUrl { get; set; }

        // Url for an image of the film's poster.
        public string PosterUrl { get; set; }

        // Determines if film can be displayed to those other than the User.
        public bool IsPrivate { get; set; }

        // Is 3D an option?
        public bool Has3D { get; set; }
    }
}
