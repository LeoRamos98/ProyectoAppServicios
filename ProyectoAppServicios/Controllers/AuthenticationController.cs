using AppServicios.Application.DTOs.Autenticacion;
using AppServicios.Application.DTOs.Usuarios;
using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthenticationController : ControladorAutenticado
    {
        private readonly IAutenticacionService _autenticacionService;
        private readonly IUsuarioService _usuarioService;

        public AuthenticationController(
            IAutenticacionService autenticacionService,
            IUsuarioService usuarioService)
        {
            _autenticacionService = autenticacionService;
            _usuarioService = usuarioService;
        }

        // POST api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            var resultado = await _autenticacionService
                .IniciarSesionAsync(request, cancellationToken);

            // Mismo mensaje para email inexistente, contraseña incorrecta o
            // cuenta inactiva, para no revelar qué emails están registrados.
            return resultado is null
                ? Problem(
                    detail: "Email o contraseña incorrectos.",
                    statusCode: StatusCodes.Status401Unauthorized)
                : Ok(resultado);
        }

        // GET api/auth/yo  (datos del usuario dueño del token)
        [HttpGet("yo")]
        [Authorize]
        [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UsuarioResponse>> Yo(
            CancellationToken cancellationToken)
        {
            if (!IntentarObtenerIdUsuario(out var idUsuario))
            {
                return Challenge();
            }

            var usuario = await _usuarioService
                .ObtenerPorIdAsync(idUsuario, cancellationToken);

            // El token sigue siendo válido hasta que vence, pero la cuenta
            // pudo haberse dado de baja después de emitirlo.
            if (usuario is null || !usuario.Activo)
            {
                return Forbid();
            }

            return Ok(usuario);
        }
    }
}