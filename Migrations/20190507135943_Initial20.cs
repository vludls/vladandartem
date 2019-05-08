using Microsoft.EntityFrameworkCore.Migrations;

namespace vladandartem.Migrations
{
    public partial class Initial20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "ff12c527-f733-495b-9788-ef50372a80ca");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "544c2bd2-ea5c-43b9-a1bc-87f33a66f72f");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "69755501-0e1e-4301-93ec-b34f99b9fd48", "AQAAAAEAACcQAAAAEA7N/Y7IGJBPZJ407NJUjUIjaF/NVVJ6A4Yq5/soTwgRgNA0CeIeytfVKfPOy9ofUw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "97109921-b9ad-4449-94a5-b9236431ea6d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "86a79a74-d373-4961-83fe-aea118f3cb5c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "81298bcd-1ecb-404e-b642-14429d76c895", "AQAAAAEAACcQAAAAEOt5paNsQLXB7HLkdWfJvA7ba6sTueBOF4ThN8EuPRATzPegG6qKPETyhNU16Q+HGg==" });
        }
    }
}
