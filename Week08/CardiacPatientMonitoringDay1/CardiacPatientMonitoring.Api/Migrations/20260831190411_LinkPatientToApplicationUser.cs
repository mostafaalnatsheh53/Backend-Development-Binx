using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardiacPatientMonitoring.Api.Migrations
{
    /// <inheritdoc />
    public partial class LinkPatientToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a3d32497-2d0d-43cb-b1d3-660939cfe1d3", 0, "215efe58-17b6-49f0-a59e-5063c5043d69", "alex.taylor@example.invalid", true, false, null, "ALEX.TAYLOR@EXAMPLE.INVALID", "ALEX.TAYLOR@EXAMPLE.INVALID", null, null, false, "f0525098-66e0-4272-b847-65c6354c868a", false, "alex.taylor@example.invalid" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: "a3d32497-2d0d-43cb-b1d3-660939cfe1d3");

            migrationBuilder.Sql(@"
DECLARE @LegacyUsers TABLE (PatientId int NOT NULL, UserId nvarchar(450) NOT NULL);

INSERT INTO @LegacyUsers (PatientId, UserId)
SELECT Id, CONVERT(nvarchar(450), NEWID())
FROM Patients
WHERE UserId = N'';

INSERT INTO AspNetUsers
    (Id, AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed,
     LockoutEnabled, LockoutEnd, NormalizedEmail, NormalizedUserName,
     PasswordHash, PhoneNumber, PhoneNumberConfirmed, SecurityStamp,
     TwoFactorEnabled, UserName)
SELECT
    UserId, 0, CONVERT(nvarchar(450), NEWID()),
    CONCAT(N'legacy.patient', PatientId, N'@example.invalid'), 1,
    0, NULL,
    CONCAT(N'LEGACY.PATIENT', PatientId, N'@EXAMPLE.INVALID'),
    CONCAT(N'LEGACY.PATIENT', PatientId, N'@EXAMPLE.INVALID'),
    NULL, NULL, 0, CONVERT(nvarchar(450), NEWID()), 0,
    CONCAT(N'legacy.patient', PatientId, N'@example.invalid')
FROM @LegacyUsers;

UPDATE Patients
SET UserId = legacy.UserId
FROM Patients
INNER JOIN @LegacyUsers AS legacy ON legacy.PatientId = Patients.Id;");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_UserId",
                table: "Patients");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a3d32497-2d0d-43cb-b1d3-660939cfe1d3");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Patients");
        }
    }
}
