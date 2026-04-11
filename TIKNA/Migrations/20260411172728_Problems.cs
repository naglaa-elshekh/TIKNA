using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class Problems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7bb7eac9-d904-445c-9c36-323c903f0dbc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1931321-e11f-478b-8baf-cee4b1697e6d");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5822fdab-dcff-4556-aa80-61c41300c48b", "1fa95c67-3569-4b23-bfaa-af8593e5f549" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5822fdab-dcff-4556-aa80-61c41300c48b");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1fa95c67-3569-4b23-bfaa-af8593e5f549");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "5822fdab-dcff-4556-aa80-61c41300c48b", null, "Admin", "ADMIN" },
                    { "7bb7eac9-d904-445c-9c36-323c903f0dbc", null, "Student", "STUDENT" },
                    { "b1931321-e11f-478b-8baf-cee4b1697e6d", null, "Company", "COMPANY" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ApprovalStatus", "Bio", "CommercialRegister", "CompanyServiceType", "ConcurrencyStamp", "Email", "EmailConfirmed", "Faculty", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "University", "UserName", "UserType" },
                values: new object[] { "1fa95c67-3569-4b23-bfaa-af8593e5f549", 0, "Main Admin Office", "Approved", null, null, null, "a53263b2-33d8-4275-b1cf-3d80819ddf61", "admin@tikna.com", true, null, false, null, "System Admin", "ADMIN@TIKNA.COM", "ADMIN@TIKNA.COM", "AQAAAAIAAYagAAAAEN5AFS5ugznIyqBe8BneQqKUnuzjeHKLuozQZNPxjw52tsqMOoMf6oqpAa4dq6rVSw==", null, false, "869f84d2-3c01-4ded-94d4-4e3f268251f1", false, null, "admin@tikna.com", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "5822fdab-dcff-4556-aa80-61c41300c48b", "1fa95c67-3569-4b23-bfaa-af8593e5f549" });
        }
    }
}
