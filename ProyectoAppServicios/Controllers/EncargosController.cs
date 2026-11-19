using AppServicios.Application.DTOs.Encargos;
using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType<IReadOnlyList<EncargoResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<EncargoResponse>>> ObtenerTodos(
            CancellationToken cancellationToken)
        {
            return Ok(await _encargoService.ObtenerTodosAsync(cancellationToken));
        }

        // GET api/encargos/mios Encargos donde soy solicitante o profesional
        [HttpGet("mios")]
        [ProducesResponseType<IReadOnlyList<EncargoResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public Task<ActionResult<IReadOnlyList<EncargoResponse>>> ObtenerMios(
            CancellationToken cancellationToken) =>
            EjecutarAsync(usuario =>
                _encargoService.ObtenerMiosAsync(usuario, cancellationToken));

        // GET api/encargos/5
        [HttpGet("{id:int}")]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public Task<ActionResult<EncargoResponse>> ObtenerPorId(
            int id,
            CancellationToken cancellationToken) =>
            EjecutarAsync(async usuario =>
                await _encargoService.ObtenerPorIdAsync(id, usuario, cancellationToken)
                    ?? throw new KeyNotFoundException("El encargo no existe."));

        // POST api/encargos A partir de un presupuesto aprobado por ambas partes
        [HttpPost]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public Task<ActionResult<EncargoResponse>> Crear(
            [FromBody] CrearEncargoRequest request,
            CancellationToken cancellationToken) =>
            EjecutarAsync(async usuario =>
                await _encargoService.CrearAsync(usuario, request, cancellationToken)
                    ?? throw new KeyNotFoundException("El presupuesto no existe."),
                StatusCodes.Status201Created);

        // PUT api/encargos/5/iniciar
        [HttpPut("{id:int}/iniciar")]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public Task<ActionResult<EncargoResponse>> Iniciar(
            int id,
            CancellationToken cancellationToken) =>
            EjecutarAsync(async usuario =>
                await _encargoService.IniciarAsync(id, usuario, cancellationToken)
                    ?? throw new KeyNotFoundException("El encargo no existe."));

        // PUT api/encargos/5/finalizar
        [HttpPut("{id:int}/finalizar")]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public Task<ActionResult<EncargoResponse>> Finalizar(
            int id,
            CancellationToken cancellationToken) =>
            EjecutarAsync(async usuario =>
                await _encargoService.FinalizarAsync(id, usuario, cancellationToken)
                    ?? throw new KeyNotFoundException("El encargo no existe."));

        // PUT api/encargos/5/cancelar
        [HttpPut("{id:int}/cancelar")]
        [ProducesResponseType<EncargoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public Task<ActionResult<EncargoResponse>> Cancelar(
            int id,
            CancellationToken cancellationToken) =>
            EjecutarAsync(async usuario =>
                await _encargoService.CancelarAsync(id, usuario, cancellationToken)
                    ?? throw new KeyNotFoundException("El encargo no existe."));
    }
}