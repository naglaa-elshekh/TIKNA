using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class Problem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fbdf3fdc-7e12-472b-9018-1da1141686ce");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fbff0c42-d7cb-4a33-9475-a86040ecbc9b");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a1dfd8b0-d218-48bc-b871-0c31bf16ec89", "5c29a469-f6ad-46f2-94e8-555677cc27b1" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a1dfd8b0-d218-48bc-b871-0c31bf16ec89");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5c29a469-f6ad-46f2-94e8-555677cc27b1");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a1dfd8b0-d218-48bc-b871-0c31bf16ec89", null, "Admin", "ADMIN" },
                    { "fbdf3fdc-7e12-472b-9018-1da1141686ce", null, "Student", "STUDENT" },
                    { "fbff0c42-d7cb-4a33-9475-a86040ecbc9b", null, "Company", "COMPANY" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ApprovalStatus", "Bio", "CommercialRegister", "CompanyServiceType", "ConcurrencyStamp", "Email", "EmailConfirmed", "Faculty", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "University", "UserName", "UserType" },
                values: new object[] { "5c29a469-f6ad-46f2-94e8-555677cc27b1", 0, "Main Admin Office", "Approved", null, null, null, "1d1c49f6-8f26-48aa-887b-68497f4b3234", "admin@tikna.com", true, null, false, null, "System Admin", "ADMIN@TIKNA.COM", "ADMIN@TIKNA.COM", "AQAAAAIAAYagAAAAEKzmjo7eP9TGdhM/bXCfrqqdHELoG0jbO8mgn299JPFWKBGU1gy7ceZpUx+qbnVZ1A==", null, false, "37948cd5-7078-488b-814f-437f1dbac941", false, null, "admin@tikna.com", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a1dfd8b0-d218-48bc-b871-0c31bf16ec89", "5c29a469-f6ad-46f2-94e8-555677cc27b1" });
        }
    }
}
