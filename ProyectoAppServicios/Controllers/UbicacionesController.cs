using System.Security.Claims;
using AppServicios.Application.DTOs.Ubicaciones;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/ubicaciones")]
    public class UbicacionesController : ControllerBase
    {
        private readonly IUbicacionService _ubicaciones;
        private readonly IUsuarioService _usuarios;

        public UbicacionesController(IUbicacionService ubicaciones, IUsuarioService usuarios)
        {
            _ubicaciones = ubicaciones;
            _usuarios = usuarios;
        }

        [HttpGet]
        [ProducesResponseType<IReadOnlyList<UbicacionResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IReadOnlyList<UbicacionResponse>>> Buscar(
            [FromQuery] BuscarUbicacionRequest consulta, CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idUsuario) || idUsuario <= 0)
                return Challenge();

            var usuario = await _usuarios.ObtenerPorIdAsync(idUsuario, cancellationToken);
            if (usuario is null || !usuario.Activo)
                return Forbid();

            try
            {
                if (!string.IsNullOrWhiteSpace(consulta.Direccion))
                    return Ok(await _ubicaciones.BuscarPorDireccionAsync(consulta.Direccion, cancellationToken));

                return Ok(await _ubicaciones.BuscarPorCoordenadasAsync(
                    consulta.Latitud!.Value, consulta.Longitud!.Value, cancellationToken));
            }
            catch (ServicioUbicacionException ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
