using AppServicios.Application.DTOs.Encargos;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoAppServicios.Autorizacion;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/encargos")]
    [Produces("application/json")]
    public class EncargosController : ControladorAutenticado
    {
        private readonly IEncargoService _encargoService;

        public EncargosController(IEncargoService encargoService)
        {
            _encargoService = encargoService;
        }

        // GET api/encargos
        [HttpGet]
        [Authorize(Policy = nameof(AlcanceCuenta.Administrador))]
        [ProducesResponseType<IReadOnlyList<EncargoResponse>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EncargoResponse>>> ObtenerTodos(
            CancellationToken cancellationToken)
        {
            var encargos = await _encargoService.ObtenerTodosAsync(cancellationToken);
            return Ok(encargos);
        }

        // GET api/encargos/5
        [HttpGet("{id:int}")]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EncargoResponse>> ObtenerPorId(
            int id,
            CancellationToken cancellationToken)
        {
            if (!IntentarObtenerIdUsuario(out var actor)) return Challenge();
            try
            {
                var encargo = await _encargoService.ObtenerPorIdAsync(id, actor, cancellationToken);
                return encargo is null ? NotFound() : Ok(encargo);
            }
            catch (AccesoDenegadoException) { return Forbid(); }
        }

        // GET api/encargos/usuario/5  (encargos que solicitó un usuario)
        [HttpGet("usuario/{id:int}")]
        [Authorize(Policy = nameof(AlcanceCuenta.TitularOAdministrador))]
        [ProducesResponseType<IReadOnlyList<EncargoResponse>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EncargoResponse>>> ObtenerPorUsuario(
            int id,
            CancellationToken cancellationToken)
        {
            var encargos = await _encargoService
                .ObtenerPorUsuarioAsync(id, cancellationToken);
            return Ok(encargos);
        }

        // GET api/encargos/profesional/5  (encargos asignados a un profesional)
        [HttpGet("profesional/{id:int}")]
        [Authorize(Policy = nameof(AlcanceCuenta.TitularOAdministrador))]
        [ProducesResponseType<IReadOnlyList<EncargoResponse>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EncargoResponse>>> ObtenerPorProfesional(
            int id,
            CancellationToken cancellationToken)
        {
            var encargos = await _encargoService
                .ObtenerPorProfesionalAsync(id, cancellationToken);
            return Ok(encargos);
        }

        // POST api/encargos  (se genera a partir de un presupuesto aprobado por ambas partes)
        [HttpPost]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<EncargoResponse>> Crear(
            [FromBody] CrearEncargoRequest request,
            CancellationToken cancellationToken)
        {
            if (!IntentarObtenerIdUsuario(out var actor)) return Challenge();
            try
            {
                var encargo = await _encargoService.CrearAsync(actor, request, cancellationToken);
                return encargo is null
                    ? NotFound()
                    : CreatedAtAction(
                        nameof(ObtenerPorId), new { id = encargo.Id }, encargo);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (ConflictoException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
        }

        // PUT api/encargos/5/iniciar
        [HttpPut("{id:int}/iniciar")]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<EncargoResponse>> Iniciar(
            int id,
            CancellationToken cancellationToken)
        {
            if (!IntentarObtenerIdUsuario(out var actor)) return Challenge();
            try
            {
                var encargo = await _encargoService.IniciarAsync(id, actor, cancellationToken);
                return encargo is null ? NotFound() : Ok(encargo);
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

        // PUT api/encargos/5/finalizar
        [HttpPut("{id:int}/finalizar")]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<EncargoResponse>> Finalizar(
            int id,
            CancellationToken cancellationToken)
        {
            if (!IntentarObtenerIdUsuario(out var actor)) return Challenge();
            try
            {
                var encargo = await _encargoService.FinalizarAsync(id, actor, cancellationToken);
                return encargo is null ? NotFound() : Ok(encargo);
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

        // PUT api/encargos/5/cancelar
        [HttpPut("{id:int}/cancelar")]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<EncargoResponse>> Cancelar(
            int id,
            CancellationToken cancellationToken)
        {
            if (!IntentarObtenerIdUsuario(out var actor)) return Challenge();
            try
            {
                var encargo = await _encargoService.CancelarAsync(id, actor, cancellationToken);
                return encargo is null ? NotFound() : Ok(encargo);
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
