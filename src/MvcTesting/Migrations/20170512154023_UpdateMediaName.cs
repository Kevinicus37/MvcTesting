using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MvcTesting.Migrations
{
    public partial class UpdateMediaName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_MediaTypes_MediaID",
                table: "Films");

            migrationBuilder.DropTable(
                name: "MediaTypes");

            migrationBuilder.CreateTable(
                name: "MediaFormats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFormats", x => x.ID);
                });

            migrationBuilder.AddColumn<bool>(
                name: "Has3D",
                table: "Films",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Films_MediaFormats_MediaID",
                table: "Films",
                column: "MediaID",
                principalTable: "MediaFormats",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_MediaFormats_MediaID",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Has3D",
                table: "Films");

            migrationBuilder.DropTable(
                name: "MediaFormats");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Films_MediaTypes_MediaID",
                table: "Films",
                column: "MediaID",
                principalTable: "MediaTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
