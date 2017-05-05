﻿using System;
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
        public string Format { get; set; }

        // Aspect ratio of film (1.33:1, 1.77:1, 1.85:1, 16:9, 2.35:1, 2.39:1, 2.4:1, shifting, etc.)
        public string AspectRatio { get; set; }

        // Film Id on TMDb.org
        public int TMDbId { get; set; }

        // Thoughts a user has on the movie
        public string Comments { get; set; }

        // Rating (1-10) that the user gives the movie.
        public int Rating { get; set; }

        // Director(s) of the movie
        public IList<string> Directors { get; set; }

        // Cast Members
        public IList<string> Cast { get; set; }

        // Audio Format (Dolby Digital, DTS, Dolby True HD, DTS-MA HD, Dolby Atmos, DTS-X, Auro, Stereo, Mono, etc.)
        public string AudioFormat { get; set; }

        // A general description of what the movie is about.
        public string Overview { get; set; }

        // Distributor of the film
        public string Distributor { get; set; }

        // Genres the film belongs to.
        public IList<string> Genres { get; set; }

        // Url for the film's trailer on Youtube.com
        public string TrailerUrl { get; set; }

        // Url for an image of the film's poster.
        public string PosterUrl { get; set; }
    }
}
