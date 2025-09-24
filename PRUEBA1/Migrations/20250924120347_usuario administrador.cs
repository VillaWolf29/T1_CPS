using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRUEBA1.Migrations
{
    /// <inheritdoc />
    public partial class usuarioadministrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "IdUsuario", "Apellido", "Contraseña", "Correo", "IdRol", "Nombre" },
                values: new object[] { 1, "Llovera", "$2a$11$ftbwkevIm.kVrZCJtWwVFOnz8EICF6isvJeKp0ZIC.dCNM0t6ZHti", "admin@gmail.com", 1, "Abed" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuario",
                keyColumn: "IdUsuario",
                keyValue: 1);
        }
    }
}
