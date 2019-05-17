using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeLib.Migrations
{
    public partial class AddNewColumnInBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathArchive",
                table: "Books",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PathBook",
                table: "Books",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathArchive",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PathBook",
                table: "Books");
        }
    }
}
