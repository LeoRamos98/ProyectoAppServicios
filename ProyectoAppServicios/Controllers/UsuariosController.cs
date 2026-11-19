using AppServicios.Application.DTOs.Usuarios;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Produces("application/json")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET api/usuarios
        [HttpGet]
        [ProducesResponseType<IReadOnlyList<UsuarioResponse>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<UsuarioResponse>>> ObtenerTodos(
            CancellationToken cancellationToken)
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync(cancellationToken);
            return Ok(usuarios);
        }

        // GET api/usuarios/5
        [HttpGet("{id:int}")]
        [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioResponse>> ObtenerPorId(
            int id,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id, cancellationToken);
            return usuario is null ? NotFound() : Ok(usuario);
        }

        // POST api/usuarios  (registro)
        [HttpPost]
        [ProducesResponseType<UsuarioResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UsuarioResponse>> Crear(
            [FromBody] CrearUsuarioRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await _usuarioService.CrearAsync(request, cancellationToken);
                return CreatedAtAction(
                    nameof(ObtenerPorId), new { id = usuario.Id }, usuario);
            }
            catch (ConflictoException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
        }

        // PUT api/usuarios/5
        [HttpPut("{id:int}")]
        [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioResponse>> Actualizar(
            int id,
            [FromBody] ActualizarUsuarioRequest request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioService.ActualizarAsync(id, request, cancellationToken);
            return usuario is null ? NotFound() : Ok(usuario);
        }

        // DELETE api/usuarios/5  (baja lógica: Activo = false)
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Desactivar(
            int id,
            CancellationToken cancellationToken)
        {
            var desactivado = await _usuarioService.DesactivarAsync(id, cancellationToken);
            return desactivado ? NoContent() : NotFound();
        }
    }
}
