using System.Security.Claims;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoAppServicios.Autorizacion;

internal enum AlcanceCuenta { Activa, Administrador, TitularOAdministrador }

internal sealed record PermisoCuenta(AlcanceCuenta Alcance) : IAuthorizationRequirement;

internal sealed class AutorizacionCuenta(IUsuarioService usuarios) : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext contexto)
    {
        var permisos = contexto.PendingRequirements.OfType<PermisoCuenta>().ToArray();
        if (permisos.Length == 0 || contexto.Resource is not HttpContext http ||
            !int.TryParse(contexto.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) || id <= 0)
            return;

        var rol = await usuarios.ObtenerRolActivoAsync(id, http.RequestAborted);
        if (rol is null)
            return;

        var esTitular = int.TryParse(http.Request.RouteValues["id"]?.ToString(), out var titular) && titular == id;
        foreach (var permiso in permisos)
        {
            if (permiso.Alcance == AlcanceCuenta.Activa || rol == RolEnum.Admin ||
                (permiso.Alcance == AlcanceCuenta.TitularOAdministrador && esTitular))
                contexto.Succeed(permiso);
        }
    }
}
