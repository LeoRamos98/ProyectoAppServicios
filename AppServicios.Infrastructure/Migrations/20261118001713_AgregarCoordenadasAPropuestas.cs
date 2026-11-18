using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppServicios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCoordenadasAPropuestas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitud",
                table: "Propuestas",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitud",
                table: "Propuestas",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "Propuestas");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "Propuestas");
        }
    }
}
