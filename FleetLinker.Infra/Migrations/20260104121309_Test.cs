using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FleetLinker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "06c9d0e1-12f3-4a4b-ac5d-7e7f8a9b0c1d", "06c9d0e1-12f3-4a4b-ac5d-7e7f8a9b0c1d", "driver", "DRIVER" },
                    { "17d0e1f2-23a4-4b5c-bd6e-8f8a9b0c1d2e", "17d0e1f2-23a4-4b5c-bd6e-8f8a9b0c1d2e", "mechanical", "MECHANICAL" },
                    { "78f3e2f5-8d5a-4b2a-a9f1-0c5e87a4d5b1", "78f3e2f5-8d5a-4b2a-a9f1-0c5e87a4d5b1", "Admin", "ADMIN" },
                    { "b1d4e5f6-c7a8-4b9c-0d1e-2f2a3b4c5d6e", "b1d4e5f6-c7a8-4b9c-0d1e-2f2a3b4c5d6e", "Visitor", "VISITOR" },
                    { "d3f6a7b8-e9c0-4d1e-bf2a-4b4c5d6e7f8a", "d3f6a7b8-e9c0-4d1e-bf2a-4b4c5d6e7f8a", "Equipment owner", "EQUIPMENT OWNER" },
                    { "e4a7b8c9-f0d1-4e2f-8a3b-5c5d6e7f8a9b", "e4a7b8c9-f0d1-4e2f-8a3b-5c5d6e7f8a9b", "Supplier", "SUPPLIER" },
                    { "f5b8c9d0-01e2-4f3a-9b4c-6d6e7f8a9b0c", "f5b8c9d0-01e2-4f3a-9b4c-6d6e7f8a9b0c", "Maintenance workshop owner", "MAINTENANCE WORKSHOP OWNER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "06c9d0e1-12f3-4a4b-ac5d-7e7f8a9b0c1d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "17d0e1f2-23a4-4b5c-bd6e-8f8a9b0c1d2e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "78f3e2f5-8d5a-4b2a-a9f1-0c5e87a4d5b1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1d4e5f6-c7a8-4b9c-0d1e-2f2a3b4c5d6e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d3f6a7b8-e9c0-4d1e-bf2a-4b4c5d6e7f8a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e4a7b8c9-f0d1-4e2f-8a3b-5c5d6e7f8a9b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f5b8c9d0-01e2-4f3a-9b4c-6d6e7f8a9b0c");
        }
    }
}
