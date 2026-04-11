using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class EditProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_OwnerId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "05eac34f-9475-4ea8-b37b-ecb2c5e5551c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "86923eae-1054-44e8-9e29-3edda0fb8be3");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b54ca175-fff2-4962-bfa3-606634955933", "ed25bb58-23a6-476a-aee7-4519ae1013a6" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b54ca175-fff2-4962-bfa3-606634955933");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ed25bb58-23a6-476a-aee7-4519ae1013a6");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Products",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_OwnerId",
                table: "Products",
                newName: "IX_Products_ApplicationUserId");

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
                name: "FK_Products_AspNetUsers_ApplicationUserId",
                table: "Products",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_ApplicationUserId",
                table: "Products");

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
                name: "ApplicationUserId",
                table: "Products",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ApplicationUserId",
                table: "Products",
                newName: "IX_Products_OwnerId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "05eac34f-9475-4ea8-b37b-ecb2c5e5551c", null, "Student", "STUDENT" },
                    { "86923eae-1054-44e8-9e29-3edda0fb8be3", null, "Company", "COMPANY" },
                    { "b54ca175-fff2-4962-bfa3-606634955933", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ApprovalStatus", "Bio", "CommercialRegister", "CompanyServiceType", "ConcurrencyStamp", "Email", "EmailConfirmed", "Faculty", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "University", "UserName", "UserType" },
                values: new object[] { "ed25bb58-23a6-476a-aee7-4519ae1013a6", 0, "Main Admin Office", "Approved", null, null, null, "3f89a768-0b3d-4124-8efb-9059c4cf202f", "admin@tikna.com", true, null, false, null, "System Admin", "ADMIN@TIKNA.COM", "ADMIN@TIKNA.COM", "AQAAAAIAAYagAAAAEK7RXO7UHcrU46JDShbs1FxqbKo6VnVEgLDFvcXFc/RqPH0Rov55nXLDVKlcKQ6Uww==", null, false, "d2304a77-ef99-4012-a1a5-eec825bef77f", false, null, "admin@tikna.com", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "b54ca175-fff2-4962-bfa3-606634955933", "ed25bb58-23a6-476a-aee7-4519ae1013a6" });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_OwnerId",
                table: "Products",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
