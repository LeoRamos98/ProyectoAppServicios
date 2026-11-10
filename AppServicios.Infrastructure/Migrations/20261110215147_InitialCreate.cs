using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppServicios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contrasenia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profesionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Zona = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoProfesional = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NroMatricula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComentarioPresentacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesionales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profesionales_Usuarios_Id",
                        column: x => x.Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Propuestas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    TipoServicio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoPropuesta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaSolicitada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propuestas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Propuestas_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Presupuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProfesional = table.Column<int>(type: "int", nullable: false),
                    IdPropuesta = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPresupuesto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AprobadoUsuario = table.Column<bool>(type: "bit", nullable: false),
                    AprobadoProfesional = table.Column<bool>(type: "bit", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presupuestos_Profesionales_IdProfesional",
                        column: x => x.IdProfesional,
                        principalTable: "Profesionales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Presupuestos_Propuestas_IdPropuesta",
                        column: x => x.IdPropuesta,
                        principalTable: "Propuestas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Encargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPropuesta = table.Column<int>(type: "int", nullable: false),
                    IdProfesionalPropuesta = table.Column<int>(type: "int", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaInicioEncargo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFinalizacionEncargo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encargos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Encargos_Presupuestos_IdProfesionalPropuesta",
                        column: x => x.IdProfesionalPropuesta,
                        principalTable: "Presupuestos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Encargos_Propuestas_IdPropuesta",
                        column: x => x.IdPropuesta,
                        principalTable: "Propuestas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Calificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEncargo = table.Column<int>(type: "int", nullable: false),
                    IdProfesional = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Puntaje = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCalificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Encargos_IdEncargo",
                        column: x => x.IdEncargo,
                        principalTable: "Encargos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Calificaciones_Profesionales_IdProfesional",
                        column: x => x.IdProfesional,
                        principalTable: "Profesionales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Calificaciones_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEncargo = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdProfesional = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_Encargos_IdEncargo",
                        column: x => x.IdEncargo,
                        principalTable: "Encargos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Chats_Profesionales_IdProfesional",
                        column: x => x.IdProfesional,
                        principalTable: "Profesionales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Chats_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Mensajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChat = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensajes_Chats_IdChat",
                        column: x => x.IdChat,
                        principalTable: "Chats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mensajes_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdEncargo",
                table: "Calificaciones",
                column: "IdEncargo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdProfesional",
                table: "Calificaciones",
                column: "IdProfesional");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdUsuario",
                table: "Calificaciones",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_IdEncargo",
                table: "Chats",
                column: "IdEncargo");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_IdProfesional",
                table: "Chats",
                column: "IdProfesional");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_IdUsuario",
                table: "Chats",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Encargos_IdProfesionalPropuesta",
                table: "Encargos",
                column: "IdProfesionalPropuesta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Encargos_IdPropuesta",
                table: "Encargos",
                column: "IdPropuesta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_IdChat",
                table: "Mensajes",
                column: "IdChat");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_IdUsuario",
                table: "Mensajes",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuestos_IdProfesional_IdPropuesta",
                table: "Presupuestos",
                columns: new[] { "IdProfesional", "IdPropuesta" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Presupuestos_IdPropuesta",
                table: "Presupuestos",
                column: "IdPropuesta");

            migrationBuilder.CreateIndex(
                name: "IX_Propuestas_IdUsuario",
                table: "Propuestas",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calificaciones");

            migrationBuilder.DropTable(
                name: "Mensajes");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Encargos");

            migrationBuilder.DropTable(
                name: "Presupuestos");

            migrationBuilder.DropTable(
                name: "Profesionales");

            migrationBuilder.DropTable(
                name: "Propuestas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
