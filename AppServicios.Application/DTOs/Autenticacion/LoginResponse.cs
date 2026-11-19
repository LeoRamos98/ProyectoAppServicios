using AppServicios.Application.DTOs.Usuarios;

namespace AppServicios.Application.DTOs.Autenticacion
{
    /// <summary>
    /// Resultado de un inicio de sesión correcto. El cliente envía el token
    /// en cada solicitud con el encabezado "Authorization: Bearer {Token}".
    /// </summary>
    public record LoginResponse(
        string Token,
        DateTime ExpiraEn,
        UsuarioResponse Usuario);
}