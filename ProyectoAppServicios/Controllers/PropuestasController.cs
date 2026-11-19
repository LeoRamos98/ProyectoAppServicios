using System.Security.Claims;
using AppServicios.Application.DTOs.Propuestas;
using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/propuestas")]
    public class PropuestasController : ControllerBase
    {
        private readonly IPropuestaService _propuestaService;

        public PropuestasController(IPropuestaService propuestaService)
        {
            _propuestaService = propuestaService;
        }

        [HttpPost]
        [ProducesResponseType<PropuestaResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PropuestaResponse>> Crear(
            [FromBody] CrearPropuestaRequest request,
            CancellationToken cancellationToken)
        {
            if (!IntentarObtenerIdUsuario(out var idUsuario))
            {
                return Challenge();
            }

            var propuesta = await _propuestaService.CrearAsync(
                idUsuario, request, cancellationToken);

            if (propuesta is null)
            {
                return Forbid();
            }

            return StatusCode(StatusCodes.Status201Created, propuesta);
        }

        [HttpGet("activas")]
        [ProducesResponseType<IReadOnlyList<PropuestaResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<PropuestaResponse>>> ObtenerActivas(
            CancellationToken cancellationToken)
        {
            if (!IntentarObtenerIdUsuario(out var idUsuario))
            {
                return Challenge();
            }

            var propuestas = await _propuestaService.ObtenerActivasAsync(
                idUsuario, cancellationToken);

            if (propuestas is null)
            {
                return Forbid();
            }

            return Ok(propuestas);
        }

        private bool IntentarObtenerIdUsuario(out int idUsuario)
        {
            var identificador = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(identificador, out idUsuario) && idUsuario > 0;
        }
    }
}