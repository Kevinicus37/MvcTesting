using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.Models
{
    public class MovieCollectorContext : DbContext
    {

        public MovieCollectorContext(DbContextOptions<MovieCollectorContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilmGenre>().HasKey(f => new { f.FilmID, f.GenreID });
        }

        public DbSet<Film> Films { get; set; }
        public DbSet<AudioFormat> AudioFormats { get; set; }
        public DbSet<MediaFormat> MediaFormats { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<FilmGenre> FilmGenres { get; set; }

    }
}
