using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class FinalDatabaseUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Products_ProductId",
                table: "Rentals");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4ec096d4-b7a1-4366-bbe0-d53c6164c026");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b567f787-8f4e-4aa7-976e-d18c3c649b50");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "bd9cfbf2-37a3-4ea4-9ccc-9d46a7fb9b7d", "b91f467f-0059-4b0b-9e7f-26441fef1696" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd9cfbf2-37a3-4ea4-9ccc-9d46a7fb9b7d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b91f467f-0059-4b0b-9e7f-26441fef1696");

            migrationBuilder.RenameColumn(
                name: "IssueDescription",
                table: "MaintenanceRequests",
                newName: "ProblemDescription");

            migrationBuilder.RenameColumn(
                name: "MaintenanceRequestId",
                table: "MaintenanceRequests",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MaintenanceRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "MaintenanceRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedCost",
                table: "MaintenanceRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDeliveryDate",
                table: "MaintenanceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelName",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RentalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalRequests_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2e268fab-0593-4b24-a7f7-2b201fbe78cb", null, "Admin", "ADMIN" },
                    { "6303a8ae-bfc6-4a5b-a360-d4d9ee6b967f", null, "Student", "STUDENT" },
                    { "70c4cbfe-174b-4450-87a9-a207b64322ee", null, "Company", "COMPANY" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ApprovalStatus", "Bio", "CommercialRegister", "CompanyServiceType", "ConcurrencyStamp", "Email", "EmailConfirmed", "Faculty", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "University", "UserName", "UserType" },
                values: new object[] { "dcd3ddfa-bf76-4100-90c5-208468396f98", 0, "Main Admin Office", "Approved", null, null, null, "95f6c1d7-8d44-480f-adba-f64ff0df4032", "admin@tikna.com", true, null, false, null, "System Admin", "ADMIN@TIKNA.COM", "ADMIN@TIKNA.COM", "AQAAAAIAAYagAAAAEGGIqiHvjDWyS1v+57HSmuCXNPUhrkHPhkbY3CavqpD2sHxuo8EJHtmMeOYvK07HIQ==", null, false, "3706ebb1-bb23-412b-aa22-d330c84b58ff", false, null, "admin@tikna.com", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "2e268fab-0593-4b24-a7f7-2b201fbe78cb", "dcd3ddfa-bf76-4100-90c5-208468396f98" });

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequests_ProductId",
                table: "RentalRequests",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequests_UserId",
                table: "RentalRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Products_ProductId",
                table: "Rentals",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Products_ProductId",
                table: "Rentals");

            migrationBuilder.DropTable(
                name: "RentalRequests");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6303a8ae-bfc6-4a5b-a360-d4d9ee6b967f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "70c4cbfe-174b-4450-87a9-a207b64322ee");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2e268fab-0593-4b24-a7f7-2b201fbe78cb", "dcd3ddfa-bf76-4100-90c5-208468396f98" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2e268fab-0593-4b24-a7f7-2b201fbe78cb");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dcd3ddfa-bf76-4100-90c5-208468396f98");

            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "EstimatedCost",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "ExpectedDeliveryDate",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "ModelName",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "MaintenanceRequests");

            migrationBuilder.RenameColumn(
                name: "ProblemDescription",
                table: "MaintenanceRequests",
                newName: "IssueDescription");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MaintenanceRequests",
                newName: "MaintenanceRequestId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "MaintenanceRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4ec096d4-b7a1-4366-bbe0-d53c6164c026", null, "Company", "COMPANY" },
                    { "b567f787-8f4e-4aa7-976e-d18c3c649b50", null, "Student", "STUDENT" },
                    { "bd9cfbf2-37a3-4ea4-9ccc-9d46a7fb9b7d", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ApprovalStatus", "Bio", "CommercialRegister", "CompanyServiceType", "ConcurrencyStamp", "Email", "EmailConfirmed", "Faculty", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "University", "UserName", "UserType" },
                values: new object[] { "b91f467f-0059-4b0b-9e7f-26441fef1696", 0, "Main Admin Office", "Approved", null, null, null, "f59d246e-7d2d-432f-96ef-6f29dcd47659", "admin@tikna.com", true, null, false, null, "System Admin", "ADMIN@TIKNA.COM", "ADMIN@TIKNA.COM", "AQAAAAIAAYagAAAAEBpzY4LQeY1zEVqfMUTIKcjyxeHMLUxT/1pe1HZ07KgrrPRa37vw7rMW6nS9jOC/nQ==", null, false, "a0a6cee9-6661-4aa2-a0a0-eb6c79595542", false, null, "admin@tikna.com", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "bd9cfbf2-37a3-4ea4-9ccc-9d46a7fb9b7d", "b91f467f-0059-4b0b-9e7f-26441fef1696" });

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Products_ProductId",
                table: "Rentals",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");
        }
    }
}
