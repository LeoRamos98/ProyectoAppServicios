using AppServicios.Application.DTOs.Profesionales;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoAppServicios.Autorizacion;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/profesionales")]
    [Produces("application/json")]
    public class ProfesionalesController : ControllerBase
    {
        private readonly IProfesionalService _profesionalService;

        public ProfesionalesController(
            IProfesionalService profesionalService)
        {
            _profesionalService = profesionalService;
        }

        // GET api/profesionales
        [HttpGet]
        [ProducesResponseType<IReadOnlyList<ProfesionalResponse>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProfesionalResponse>>> ObtenerTodos(
            CancellationToken cancellationToken)
        {
            var profesionales = await _profesionalService.ObtenerTodosAsync(cancellationToken);
            return Ok(profesionales);
        }

        // GET api/profesionales/5
        [HttpGet("{id:int}")]
        [ProducesResponseType<ProfesionalResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfesionalResponse>> ObtenerPorId(
            int id,
            CancellationToken cancellationToken)
        {
            var profesional = await _profesionalService.ObtenerPorIdAsync(id, cancellationToken);
            return profesional is null ? NotFound() : Ok(profesional);
        }

        [HttpGet("{id:int}/perfil")]
        [ProducesResponseType<PerfilProfesionalResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PerfilProfesionalResponse>> ObtenerPerfil(
            int id, CancellationToken cancellationToken)
        {
            var perfil = await _profesionalService.ObtenerPerfilAsync(id, cancellationToken);
            return perfil is null ? NotFound() : Ok(perfil);
        }

        // POST api/profesionales  (registro)
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType<ProfesionalResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProfesionalResponse>> Crear(
            [FromBody] CrearProfesionalRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var profesional = await _profesionalService.CrearAsync(request, cancellationToken);
                return CreatedAtAction(
                    nameof(ObtenerPorId), new { id = profesional.Id }, profesional);
            }
            catch (ConflictoException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
        }

        // PUT api/profesionales/5
        [HttpPut("{id:int}")]
        [Authorize(Policy = nameof(AlcanceCuenta.TitularOAdministrador))]
        [ProducesResponseType<ProfesionalResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfesionalResponse>> Actualizar(
            int id,
            [FromBody] ActualizarProfesionalRequest request,
            CancellationToken cancellationToken)
        {
            var profesional = await _profesionalService.ActualizarAsync(id, request, cancellationToken);
            return profesional is null ? NotFound() : Ok(profesional);
        }

        // PATCH api/profesionales/5/presentacion
        [HttpPatch("{id:int}/presentacion")]
        [Authorize(Policy = nameof(AlcanceCuenta.TitularOAdministrador))]
        [ProducesResponseType<ProfesionalResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfesionalResponse>> ActualizarPresentacion(
            int id,
            [FromBody] ActualizarPresentacionRequest request,
            CancellationToken cancellationToken)
        {
            var profesional = await _profesionalService
                .ActualizarPresentacionAsync(id, request, cancellationToken);
            return profesional is null ? NotFound() : Ok(profesional);
        }

        // PATCH api/profesionales/5/disponibilidad
        [HttpPatch("{id:int}/disponibilidad")]
        [Authorize(Policy = nameof(AlcanceCuenta.TitularOAdministrador))]
        [ProducesResponseType<ProfesionalResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProfesionalResponse>> CambiarDisponibilidad(
                int id,
                [FromBody] CambiarDisponibilidadRequest request,
                CancellationToken cancellationToken)
        {
            try
            {
                var profesional = await _profesionalService
                    .CambiarDisponibilidadAsync(id, request, cancellationToken);
                return profesional is null ? NotFound() : Ok(profesional);
            }
            catch (ConflictoException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
        }

        // DELETE api/profesionales/5  (baja lógica: Activo = false, Estado = DadoDeBaja)
        [HttpDelete("{id:int}")]
        [Authorize(Policy = nameof(AlcanceCuenta.Administrador))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DarDeBaja(
            int id,
            CancellationToken cancellationToken)
        {
            var dadoDeBaja = await _profesionalService.DarDeBajaAsync(id, cancellationToken);
            return dadoDeBaja ? NoContent() : NotFound();
        }
    }
}