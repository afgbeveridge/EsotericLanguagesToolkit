using Microsoft.EntityFrameworkCore.Migrations;

namespace Eso.API.Editor.Migrations
{
    public partial class DocumentationSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Documentation",
                table: "Languages",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Documentation",
                table: "Languages");
        }
    }
}
