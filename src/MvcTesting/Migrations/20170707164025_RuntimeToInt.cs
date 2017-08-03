using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcTesting.Migrations
{
    public partial class RuntimeToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TMDbId",
                table: "Films");

            migrationBuilder.AlterColumn<int>(
                name: "Runtime",
                table: "Films",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TMDbId",
                table: "Films",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Runtime",
                table: "Films",
                nullable: true);
        }
    }
}
