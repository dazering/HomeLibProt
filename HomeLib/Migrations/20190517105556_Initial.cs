﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeLib.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authtors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    MiddleName = table.Column<string>(nullable: false, defaultValue: "none"),
                    LastName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authtors", x => new { x.FirstName, x.MiddleName, x.LastName });
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Annotation = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true),
                    Isbn = table.Column<string>(nullable: true),
                    Cover = table.Column<string>(nullable: true),
                    AuthtorFirstName = table.Column<string>(nullable: false),
                    AuthtorMiddleName = table.Column<string>(nullable: false),
                    AuthtorLastName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authtors_AuthtorFirstName_AuthtorMiddleName_AuthtorLastName",
                        columns: x => new { x.AuthtorFirstName, x.AuthtorMiddleName, x.AuthtorLastName },
                        principalTable: "Authtors",
                        principalColumns: new[] { "FirstName", "MiddleName", "LastName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthtorFirstName_AuthtorMiddleName_AuthtorLastName",
                table: "Books",
                columns: new[] { "AuthtorFirstName", "AuthtorMiddleName", "AuthtorLastName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Authtors");
        }
    }
}
