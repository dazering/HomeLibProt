using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScannerService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authtors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: false, defaultValue: ""),
                    MiddleName = table.Column<string>(nullable: false, defaultValue: ""),
                    LastName = table.Column<string>(nullable: false, defaultValue: ""),
                    FullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authtors", x => x.Id);
                    table.UniqueConstraint("AK_Authtors_FirstName_MiddleName_LastName", x => new { x.FirstName, x.MiddleName, x.LastName });
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
                    PathArchive = table.Column<string>(nullable: true),
                    PathBook = table.Column<string>(nullable: true),
                    AuthtorFirstName = table.Column<string>(nullable: true),
                    AuthtorMiddleName = table.Column<string>(nullable: true),
                    AuthtorLastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authtors_AuthtorFirstName_AuthtorMiddleName_AuthtorLastName",
                        columns: x => new { x.AuthtorFirstName, x.AuthtorMiddleName, x.AuthtorLastName },
                        principalTable: "Authtors",
                        principalColumns: new[] { "FirstName", "MiddleName", "LastName" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authtors_FullName",
                table: "Authtors",
                column: "FullName");

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
