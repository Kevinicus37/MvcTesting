using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MvcTesting.Migrations
{
    public partial class InitMovie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AudioFormats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioFormats", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Films",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AspectRatio = table.Column<string>(nullable: true),
                    AudioID = table.Column<int>(nullable: false),
                    Cast = table.Column<string>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    Directors = table.Column<string>(nullable: true),
                    IsPrivate = table.Column<bool>(nullable: false),
                    MediaID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Overview = table.Column<string>(nullable: true),
                    PosterUrl = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    TMDbId = table.Column<int>(nullable: false),
                    TrailerUrl = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Films", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Films_AudioFormats_AudioID",
                        column: x => x.AudioID,
                        principalTable: "AudioFormats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Films_MediaTypes_MediaID",
                        column: x => x.MediaID,
                        principalTable: "MediaTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilmGenre",
                columns: table => new
                {
                    FilmID = table.Column<int>(nullable: false),
                    GenreID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmGenre", x => new { x.FilmID, x.GenreID });
                    table.ForeignKey(
                        name: "FK_FilmGenre_Films_FilmID",
                        column: x => x.FilmID,
                        principalTable: "Films",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmGenre_Genres_GenreID",
                        column: x => x.GenreID,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Films_AudioID",
                table: "Films",
                column: "AudioID");

            migrationBuilder.CreateIndex(
                name: "IX_Films_MediaID",
                table: "Films",
                column: "MediaID");

            migrationBuilder.CreateIndex(
                name: "IX_FilmGenre_FilmID",
                table: "FilmGenre",
                column: "FilmID");

            migrationBuilder.CreateIndex(
                name: "IX_FilmGenre_GenreID",
                table: "FilmGenre",
                column: "GenreID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmGenre");

            migrationBuilder.DropTable(
                name: "Films");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "AudioFormats");

            migrationBuilder.DropTable(
                name: "MediaTypes");
        }
    }
}
