using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaintenance3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredTimeSlot",
                table: "MaintenanceRequests");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c90040e0-8594-4b10-80cd-9eb1cbe50f5b", "AQAAAAIAAYagAAAAEO+hUFKIFNtIUQqMNUl4TPuNKI+vqgYPMTYYkpbFET5XHyUKCNyPlnTl6hVbcD+w+Q==", "4ed5774d-b9eb-492a-acdc-c0c10500d6cb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredTimeSlot",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e510ab46-d27f-4187-8fd3-40759f53545a", "AQAAAAIAAYagAAAAEEbkfOpKWoCnRNHxiBB/n2dLkYWnccUnSXLh2NtrmQiWPZ1+YOIvS1x4Ioroo1WXhA==", "893fd52c-46fb-48db-8eb1-e475a60e27e5" });
        }
    }
}
