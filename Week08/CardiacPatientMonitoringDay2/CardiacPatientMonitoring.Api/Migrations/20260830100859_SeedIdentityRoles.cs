using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CardiacPatientMonitoring.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "07abcc84-88db-460f-b3ab-8730cd53cc24", "787bf9f0-a6dc-427a-a9b3-c47ec9d145d3", "Admin", "ADMIN" },
                    { "60f03c42-071e-486e-8d00-9e289462b568", "83e95786-f6f2-4404-9f2c-c7a437e3a2eb", "Patient", "PATIENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "07abcc84-88db-460f-b3ab-8730cd53cc24");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "60f03c42-071e-486e-8d00-9e289462b568");
        }
    }
}
