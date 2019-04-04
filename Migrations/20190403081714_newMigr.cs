using Microsoft.EntityFrameworkCore.Migrations;

namespace vladandartem.Migrations
{
    public partial class newMigr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriesNew",
                table: "CategoriesNew");

            migrationBuilder.RenameTable(
                name: "CategoriesNew",
                newName: "Categories");
            */
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Manufacturer",
                table: "Products",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImgPath",
                table: "Products",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            /*migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");*/

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CartJSON = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            /*/migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "CategoriesNew");
            */
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Manufacturer",
                table: "Products",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ImgPath",
                table: "Products",
                nullable: true,
                oldClrType: typeof(string));

            /*migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriesNew",
                table: "CategoriesNew",
                column: "Id");*/
        }
    }
}
