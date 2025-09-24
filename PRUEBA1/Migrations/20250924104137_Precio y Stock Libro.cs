using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRUEBA1.Migrations
{
    /// <inheritdoc />
    public partial class PrecioyStockLibro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "Libro",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Libro",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Libro");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Libro");
        }
    }
}
