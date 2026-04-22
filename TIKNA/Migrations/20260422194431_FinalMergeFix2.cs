using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class FinalMergeFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "45786c78-6add-4cc1-b2b6-7a30c9428356" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "45786c78-6add-4cc1-b2b6-7a30c9428356");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ApprovalStatus", "Bio", "CommercialRegister", "CompanyServiceType", "ConcurrencyStamp", "Email", "EmailConfirmed", "Faculty", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "University", "UserName", "UserType" },
                values: new object[] { "7619e589-693e-433a-b48f-eb0d1d66408e", 0, "Main Admin Office", "Approved", null, null, null, "bd3f0ac9-6024-4a30-a423-a6539cd2f18d", "admin@tikna.com", true, null, false, null, "System Admin", "ADMIN@TIKNA.COM", "ADMIN@TIKNA.COM", "AQAAAAIAAYagAAAAEBfml3ALcGYpB0bCtAORwJJ9rEvA5canNrR+8coHFaEth3+AJauSkd4R0fYrVUNNGA==", null, false, "0af59b5f-d849-44c8-a1d6-cdf1c215a300", false, null, "admin@tikna.com", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "7619e589-693e-433a-b48f-eb0d1d66408e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "7619e589-693e-433a-b48f-eb0d1d66408e" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7619e589-693e-433a-b48f-eb0d1d66408e");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ApprovalStatus", "Bio", "CommercialRegister", "CompanyServiceType", "ConcurrencyStamp", "Email", "EmailConfirmed", "Faculty", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "University", "UserName", "UserType" },
                values: new object[] { "45786c78-6add-4cc1-b2b6-7a30c9428356", 0, "Main Admin Office", "Approved", null, null, null, "0c21c425-c0c5-49f3-abdb-61d063f23ed0", "admin@tikna.com", true, null, false, null, "System Admin", "ADMIN@TIKNA.COM", "ADMIN@TIKNA.COM", "AQAAAAIAAYagAAAAEO4DkInSoyhf1nVmv7jAck89OIHM/TkzF0iICH2YX8IbcO1KLXspRQaZxsktj9031Q==", null, false, "59d29541-684c-404b-8464-479827134043", false, null, "admin@tikna.com", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "45786c78-6add-4cc1-b2b6-7a30c9428356" });
        }
    }
}
