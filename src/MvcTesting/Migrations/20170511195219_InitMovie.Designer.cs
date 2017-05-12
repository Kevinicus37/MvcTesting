using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MvcTesting.Models;

namespace MvcTesting.Migrations
{
    [DbContext(typeof(MovieCollectorContext))]
    [Migration("20170511195219_InitMovie")]
    partial class InitMovie
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MvcTesting.Models.AudioFormat", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("AudioFormats");
                });

            modelBuilder.Entity("MvcTesting.Models.Film", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AspectRatio");

                    b.Property<int>("AudioID");

                    b.Property<string>("Cast");

                    b.Property<string>("Comments");

                    b.Property<string>("Directors");

                    b.Property<bool>("IsPrivate");

                    b.Property<int>("MediaID");

                    b.Property<string>("Name");

                    b.Property<string>("Overview");

                    b.Property<string>("PosterUrl");

                    b.Property<int>("Rating");

                    b.Property<int>("TMDbId");

                    b.Property<string>("TrailerUrl");

                    b.Property<int>("Year");

                    b.HasKey("ID");

                    b.HasIndex("AudioID");

                    b.HasIndex("MediaID");

                    b.ToTable("Films");
                });

            modelBuilder.Entity("MvcTesting.Models.FilmGenre", b =>
                {
                    b.Property<int>("FilmID");

                    b.Property<int>("GenreID");

                    b.HasKey("FilmID", "GenreID");

                    b.HasIndex("FilmID");

                    b.HasIndex("GenreID");

                    b.ToTable("FilmGenre");
                });

            modelBuilder.Entity("MvcTesting.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("MvcTesting.Models.MediaType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("MediaTypes");
                });

            modelBuilder.Entity("MvcTesting.Models.Film", b =>
                {
                    b.HasOne("MvcTesting.Models.AudioFormat", "Audio")
                        .WithMany("Films")
                        .HasForeignKey("AudioID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MvcTesting.Models.MediaType", "Media")
                        .WithMany("Films")
                        .HasForeignKey("MediaID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MvcTesting.Models.FilmGenre", b =>
                {
                    b.HasOne("MvcTesting.Models.Film", "Film")
                        .WithMany("Genres")
                        .HasForeignKey("FilmID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MvcTesting.Models.Genre", "Genre")
                        .WithMany("FilmGenres")
                        .HasForeignKey("GenreID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
