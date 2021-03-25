using Microsoft.EntityFrameworkCore.Migrations;

namespace Eso.API.Editor.Migrations
{
    public partial class RecogniseCustomLanguages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNativelySupported",
                table: "Languages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNativelySupported",
                table: "Languages");
        }
    }
}
