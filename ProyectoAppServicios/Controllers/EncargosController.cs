using AppServicios.Application.DTOs.Encargos;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using AppServicios.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Route("api/encargos")]
    [Produces("application/json")]
    public class EncargosController : ControllerBase
    {
        private readonly IEncargoService _encargoService;

        public EncargosController(IEncargoService encargoService)
        {
            _encargoService = encargoService;
        }

        // GET api/encargos
        [HttpGet]
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
            var encargo = await _encargoService.ObtenerPorIdAsync(id, cancellationToken);
            return encargo is null ? NotFound() : Ok(encargo);
        }

        // GET api/encargos/usuario/5  (encargos que solicitó un usuario)
        [HttpGet("usuario/{idUsuario:int}")]
        [ProducesResponseType<IReadOnlyList<EncargoResponse>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EncargoResponse>>> ObtenerPorUsuario(
            int idUsuario,
            CancellationToken cancellationToken)
        {
            var encargos = await _encargoService
                .ObtenerPorUsuarioAsync(idUsuario, cancellationToken);
            return Ok(encargos);
        }

        // GET api/encargos/profesional/5  (encargos asignados a un profesional)
        [HttpGet("profesional/{idProfesional:int}")]
        [ProducesResponseType<IReadOnlyList<EncargoResponse>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EncargoResponse>>> ObtenerPorProfesional(
            int idProfesional,
            CancellationToken cancellationToken)
        {
            var encargos = await _encargoService
                .ObtenerPorProfesionalAsync(idProfesional, cancellationToken);
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
            try
            {
                var encargo = await _encargoService.CrearAsync(request, cancellationToken);
                return encargo is null
                    ? NotFound()
                    : CreatedAtAction(
                        nameof(ObtenerPorId), new { id = encargo.Id }, encargo);
            }
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
            [FromBody] AccionEncargoRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var encargo = await _encargoService.IniciarAsync(id, request, cancellationToken);
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
            [FromBody] AccionEncargoRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var encargo = await _encargoService.FinalizarAsync(id, request, cancellationToken);
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
            [FromBody] AccionEncargoRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var encargo = await _encargoService.CancelarAsync(id, request, cancellationToken);
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
