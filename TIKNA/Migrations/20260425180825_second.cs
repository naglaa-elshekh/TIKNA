using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "ProfilePictureUrl", "SecurityStamp" },
                values: new object[] { "2d462dec-faa6-4610-84fa-53c9afe00412", "AQAAAAIAAYagAAAAEEaOHCMoSArnjyz9jCveQZ5mcE1qnH75KUseoU21l1mZWkIOLmzGGZU5UQgNLvNP2A==", null, "c87dc54f-cf69-4549-b58b-4d537011723e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "46a0a865-eb05-4ffd-90e3-91544ff370e2", "AQAAAAIAAYagAAAAEHzCoVrCQn/n0VZ7A9AS5ROLXpD356pkoA9QV3txtpD1SFNPZ05+8s7wTAIdNqPnOA==", "3cfdab1a-3348-412f-9427-936adc920982" });
        }
    }
}
