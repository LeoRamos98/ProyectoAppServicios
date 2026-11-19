using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;

namespace AppServicios.Application.DTOs.Profesionales
{
    public record ProfesionalResponse(
        int Id,
        string Nombre,
        string Apellido,
        string Email,
        string Telefono,
        string Direccion,
        RolEnum Rol,
        bool Activo,
        string Zona,
        string TipoProfesional,
        string NroMatricula,
        string FotoPerfil,
        string ComentarioPresentacion,
        EstadoProfesionalEnum Estado)
    {
        public static ProfesionalResponse DesdeEntidad(Profesional profesional) => new(
            profesional.Id,
            profesional.Nombre,
            profesional.Apellido,
            profesional.Email,
            profesional.Telefono,
            profesional.Direccion,
            profesional.Rol,
            profesional.Activo,
            profesional.Zona,
            profesional.TipoProfesional,
            profesional.NroMatricula,
            profesional.FotoPerfil,
            profesional.ComentarioPresentacion,
            profesional.Estado);
    }
}