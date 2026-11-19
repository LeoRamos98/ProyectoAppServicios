using AppServicios.Application.DTOs.Calificaciones;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/calificaciones")]
    [Produces("application/json")]
    public class CalificacionesController : ControladorAutenticado
    {
        private readonly ICalificacionService _calificacionService;

        public CalificacionesController(
            ICalificacionService calificacionService)
        {
            _calificacionService = calificacionService;
        }

        // GET api/calificaciones/5
        [HttpGet("{id:int}")]
        [ProducesResponseType<CalificacionResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CalificacionResponse>> ObtenerPorId(
            int id,
            CancellationToken cancellationToken)
        {
            var calificacion = await _calificacionService
                .ObtenerPorIdAsync(id, cancellationToken);
            return calificacion is null ? NotFound() : Ok(calificacion);
        }

        // GET api/calificaciones/profesional/5  (promedio, cantidad y detalle)
        [HttpGet("profesional/{idProfesional:int}")]
        [ProducesResponseType<CalificacionesProfesionalResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CalificacionesProfesionalResponse>> ObtenerPorProfesional(
            int idProfesional,
            CancellationToken cancellationToken)
        {
            var resultado = await _calificacionService
                .ObtenerPorProfesionalAsync(idProfesional, cancellationToken);
            return resultado is null ? NotFound() : Ok(resultado);
        }

        // GET api/encargos/5/calificacion
        [HttpGet("/api/encargos/{idEncargo:int}/calificacion")]
        [ProducesResponseType<CalificacionResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CalificacionResponse>> ObtenerPorEncargo(
            int idEncargo,
            CancellationToken cancellationToken)
        {
            var calificacion = await _calificacionService
                .ObtenerPorEncargoAsync(idEncargo, cancellationToken);
            return calificacion is null ? NotFound() : Ok(calificacion);
        }

        // POST api/encargos/5/calificacion  (el usuario califica al profesional)
        [HttpPost("/api/encargos/{idEncargo:int}/calificacion")]
        [ProducesResponseType<CalificacionResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CalificacionResponse>> CalificarEncargo(
            int idEncargo,
            [FromBody] CrearCalificacionRequest request,
            CancellationToken cancellationToken)
        {
            if (!IntentarObtenerIdUsuario(out var actor)) return Challenge();
            try
            {
                var calificacion = await _calificacionService
                    .CalificarEncargoAsync(idEncargo, actor, request, cancellationToken);

                return calificacion is null
                    ? NotFound()
                    : CreatedAtAction(
                        nameof(ObtenerPorId), new { id = calificacion.Id }, calificacion);
            }
            catch (AccesoDenegadoException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status403Forbidden);
            }
            catch (ConflictoException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
        }
    }
}
