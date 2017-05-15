using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcTesting.Migrations
{
    public partial class AddFilmGenres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilmGenre_Films_FilmID",
                table: "FilmGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_FilmGenre_Genres_GenreID",
                table: "FilmGenre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FilmGenre",
                table: "FilmGenre");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FilmGenres",
                table: "FilmGenre",
                columns: new[] { "FilmID", "GenreID" });

            migrationBuilder.AddForeignKey(
                name: "FK_FilmGenres_Films_FilmID",
                table: "FilmGenre",
                column: "FilmID",
                principalTable: "Films",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FilmGenres_Genres_GenreID",
                table: "FilmGenre",
                column: "GenreID",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Genres",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_FilmGenre_GenreID",
                table: "FilmGenre",
                newName: "IX_FilmGenres_GenreID");

            migrationBuilder.RenameIndex(
                name: "IX_FilmGenre_FilmID",
                table: "FilmGenre",
                newName: "IX_FilmGenres_FilmID");

            migrationBuilder.RenameTable(
                name: "FilmGenre",
                newName: "FilmGenres");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilmGenres_Films_FilmID",
                table: "FilmGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_FilmGenres_Genres_GenreID",
                table: "FilmGenres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FilmGenres",
                table: "FilmGenres");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FilmGenre",
                table: "FilmGenres",
                columns: new[] { "FilmID", "GenreID" });

            migrationBuilder.AddForeignKey(
                name: "FK_FilmGenre_Films_FilmID",
                table: "FilmGenres",
                column: "FilmID",
                principalTable: "Films",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FilmGenre_Genres_GenreID",
                table: "FilmGenres",
                column: "GenreID",
                principalTable: "Genres",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Genres",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_FilmGenres_GenreID",
                table: "FilmGenres",
                newName: "IX_FilmGenre_GenreID");

            migrationBuilder.RenameIndex(
                name: "IX_FilmGenres_FilmID",
                table: "FilmGenres",
                newName: "IX_FilmGenre_FilmID");

            migrationBuilder.RenameTable(
                name: "FilmGenres",
                newName: "FilmGenre");
        }
    }
}
