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
            var identificador = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(identificador, out var idUsuario) || idUsuario <= 0)
            {
                return Challenge();
            }

            var propuesta = await _propuestaService.CrearAsync(idUsuario, request, cancellationToken);
            if (propuesta is null)
            {
                return Forbid();
            }

            return StatusCode(StatusCodes.Status201Created, propuesta);
        }
    }
}