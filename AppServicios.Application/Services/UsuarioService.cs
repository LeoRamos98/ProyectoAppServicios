using AppServicios.Domain.Enums;
using AppServicios.Application.DTOs.Usuarios;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Entities;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHasherContrasenia _hasherContrasenia;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IHasherContrasenia hasherContrasenia)
        {
            _usuarioRepository = usuarioRepository;
            _hasherContrasenia = hasherContrasenia;
        }

        public Task<RolEnum?> ObtenerRolActivoAsync(int id, CancellationToken cancellationToken = default) =>
            _usuarioRepository.ObtenerRolActivoAsync(id, cancellationToken);

        public async Task<IReadOnlyList<UsuarioResponse>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            var usuarios = await _usuarioRepository
                .ObtenerTodosAsync(cancellationToken);

            return usuarios.Select(UsuarioResponse.DesdeEntidad).ToList();
        }

        public async Task<UsuarioResponse?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            return usuario is null ? null : UsuarioResponse.DesdeEntidad(usuario);
        }

        public async Task<UsuarioResponse> CrearAsync(
            CrearUsuarioRequest request,
            CancellationToken cancellationToken = default)
        {
            var email = NormalizarEmail(request.Email);

            if (await _usuarioRepository.ExisteEmailAsync(email, cancellationToken))
            {
                throw new ConflictoException(
                    $"Ya existe un usuario registrado con el email '{email}'.");
            }

            var usuario = new Usuario(
                request.Nombre.Trim(),
                request.Apellido.Trim(),
                email,
                _hasherContrasenia.Hashear(request.Contrasenia),
                request.Telefono.Trim(),
                request.Direccion.Trim());

            await _usuarioRepository.AgregarAsync(usuario, cancellationToken);
            await _usuarioRepository.GuardarCambiosAsync(cancellationToken);

            return UsuarioResponse.DesdeEntidad(usuario);
        }

        public async Task<UsuarioResponse?> ActualizarAsync(
            int id,
            ActualizarUsuarioRequest request,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            if (usuario is null)
            {
                return null;
            }

            usuario.ActualizarPerfil(
                request.Nombre.Trim(),
                request.Apellido.Trim(),
                request.Telefono.Trim(),
                request.Direccion.Trim());

            await _usuarioRepository.GuardarCambiosAsync(cancellationToken);

            return UsuarioResponse.DesdeEntidad(usuario);
        }

        public async Task<bool> DesactivarAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            if (usuario is null)
            {
                return false;
            }

            if (usuario is Profesional profesional)
                profesional.DarDeBaja();
            else
                usuario.Desactivar();
            await _usuarioRepository.GuardarCambiosAsync(cancellationToken);

            return true;
        }

        private static string NormalizarEmail(string email)
            => email.Trim().ToLowerInvariant();
    }
}
