using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaintenanceRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_ApplicationUserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_ApplicationUserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "MaintenanceRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MaintenanceRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CenterId",
                table: "MaintenanceRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d2244a8a-af28-4383-bca4-6a22e8be1347", "AQAAAAIAAYagAAAAEMDWPHjz724G2uyz5X3C+L7oQwwLVE9nBze5DcAII5+y2kKUIq4alaRvPEJQf21SXw==", "500fd356-59ba-4436-9b9b-299db109f697" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_CenterId",
                table: "MaintenanceRequests",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_UserId",
                table: "MaintenanceRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_CenterId",
                table: "MaintenanceRequests",
                column: "CenterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_UserId",
                table: "MaintenanceRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_CenterId",
                table: "MaintenanceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_UserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_CenterId",
                table: "MaintenanceRequests");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_UserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "MaintenanceRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "MaintenanceRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2d462dec-faa6-4610-84fa-53c9afe00412", "AQAAAAIAAYagAAAAEEaOHCMoSArnjyz9jCveQZ5mcE1qnH75KUseoU21l1mZWkIOLmzGGZU5UQgNLvNP2A==", "c87dc54f-cf69-4549-b58b-4d537011723e" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_ApplicationUserId",
                table: "MaintenanceRequests",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_ApplicationUserId",
                table: "MaintenanceRequests",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
