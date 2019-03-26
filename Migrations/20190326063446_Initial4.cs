using Microsoft.EntityFrameworkCore.Migrations;

namespace vladandartem.Migrations
{
    public partial class Initial4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "CategoriesNew");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriesNew",
                table: "CategoriesNew",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriesNew",
                table: "CategoriesNew");

            migrationBuilder.RenameTable(
                name: "CategoriesNew",
                newName: "Categories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");
        }
    }
}
