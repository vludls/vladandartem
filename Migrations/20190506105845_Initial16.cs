using Microsoft.EntityFrameworkCore.Migrations;

namespace vladandartem.Migrations
{
    public partial class Initial16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { 1, "50f7014b-5668-4ec5-b4f0-e2389ce95550", "user", "user" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { 2, "84f9e41b-fdf6-455c-8243-c3058e98645c", "admin", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "Year" },
                values: new object[] { 1, 0, "ee51d3ab-a39f-4d89-b646-55587eee7b8d", "Admin@yandex.ru", false, false, null, "ADMIN@YANDEX.RU", "ADMIN@YANDEX.RU", "AQAAAAEAACcQAAAAEBshpSUGttGjD8qSqQu6ey5fBBbTqDrPMVG4GFpjJPpXusbwdL7GUsLpQEd8KHzewg==", null, false, "STKLXSOQIOM7NR4EDU3TYNRP4ZHPRU7A", false, "Admin@yandex.ru", "01/01/1984" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "Cart",
                columns: new[] { "Id", "UserId" },
                values: new object[] { 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "Cart",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
