using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;

namespace AppServicios.Application.DTOs.Usuarios
{
    public record UsuarioResponse(
        int Id,
        string Nombre,
        string Apellido,
        string Email,
        string Telefono,
        string Direccion,
        RolEnum Rol,
        bool Activo)
    {
        public static UsuarioResponse DesdeEntidad(Usuario usuario) => new(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.Email,
            usuario.Telefono,
            usuario.Direccion,
            usuario.Rol,
            usuario.Activo);
    }
}
