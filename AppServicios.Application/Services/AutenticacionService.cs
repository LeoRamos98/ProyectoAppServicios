using AppServicios.Application.DTOs.Autenticacion;
using AppServicios.Application.DTOs.Usuarios;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class AutenticacionService : IAutenticacionService
    {
        // Hash con formato válido que nunca coincide. Se verifica contra él cuando
        // el email no existe, para que esa respuesta tarde lo mismo que una
        // contraseña incorrecta y no se pueda averiguar qué emails están registrados.
        private const string HashFicticio =
            "100000.AAAAAAAAAAAAAAAAAAAAAA==.AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHasherContrasenia _hasherContrasenia;
        private readonly IGeneradorToken _generadorToken;

        public AutenticacionService(
            IUsuarioRepository usuarioRepository,
            IHasherContrasenia hasherContrasenia,
            IGeneradorToken generadorToken)
        {
            _usuarioRepository = usuarioRepository;
            _hasherContrasenia = hasherContrasenia;
            _generadorToken = generadorToken;
        }

        public async Task<LoginResponse?> IniciarSesionAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var usuario = await _usuarioRepository
                .ObtenerPorEmailAsync(email, cancellationToken);

            if (usuario is null)
            {
                _hasherContrasenia.Verificar(request.Contrasenia, HashFicticio);
                return null;
            }

            if (!_hasherContrasenia.Verificar(request.Contrasenia, usuario.Contrasenia)
                || !usuario.Activo)
            {
                return null;
            }

            var token = _generadorToken.Generar(usuario);

            return new LoginResponse(
                token.Token,
                token.ExpiraEn,
                UsuarioResponse.DesdeEntidad(usuario));
        }
    }
}