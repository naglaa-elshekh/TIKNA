using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Products_ProductId",
                table: "MaintenanceRequests");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "MaintenanceRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "MaintenanceRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "MaintenanceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NoteFromCenter",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e510ab46-d27f-4187-8fd3-40759f53545a", "AQAAAAIAAYagAAAAEEbkfOpKWoCnRNHxiBB/n2dLkYWnccUnSXLh2NtrmQiWPZ1+YOIvS1x4Ioroo1WXhA==", "893fd52c-46fb-48db-8eb1-e475a60e27e5" });

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Products_ProductId",
                table: "MaintenanceRequests",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Products_ProductId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "NoteFromCenter",
                table: "MaintenanceRequests");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "MaintenanceRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d2244a8a-af28-4383-bca4-6a22e8be1347", "AQAAAAIAAYagAAAAEMDWPHjz724G2uyz5X3C+L7oQwwLVE9nBze5DcAII5+y2kKUIq4alaRvPEJQf21SXw==", "500fd356-59ba-4436-9b9b-299db109f697" });

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Products_ProductId",
                table: "MaintenanceRequests",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
