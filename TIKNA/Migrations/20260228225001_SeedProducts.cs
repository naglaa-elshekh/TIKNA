using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TIKNA.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CustomerId", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Core i7 - 16GB RAM - 512GB SSD", "Dell Inspiron 15", 32000m },
                    { 2, 1, "Core i5 - 8GB RAM - 256GB SSD", "HP Pavilion 14", 24000m },
                    { 3, 1, "Core i5 - 8GB RAM - 256GB SSD", "HP EliteBook 840", 1500m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);
        }
    }
}
