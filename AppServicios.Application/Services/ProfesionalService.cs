using AppServicios.Application.DTOs.Profesionales;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class ProfesionalService : IProfesionalService
    {
        private readonly IProfesionalRepository _profesionalRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHasherContrasenia _hasherContrasenia;

        public ProfesionalService(
            IProfesionalRepository profesionalRepository,
            IUsuarioRepository usuarioRepository,
            IHasherContrasenia hasherContrasenia)
        {
            _profesionalRepository = profesionalRepository;
            _usuarioRepository = usuarioRepository;
            _hasherContrasenia = hasherContrasenia;
        }

        public async Task<IReadOnlyList<ProfesionalResponse>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            var profesionales = await _profesionalRepository
                .ObtenerTodosAsync(cancellationToken);

            return profesionales.Select(ProfesionalResponse.DesdeEntidad).ToList();
        }

        public async Task<ProfesionalResponse?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var profesional = await _profesionalRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            return profesional is null
                ? null
                : ProfesionalResponse.DesdeEntidad(profesional);
        }

        public async Task<ProfesionalResponse> CrearAsync(
            CrearProfesionalRequest request,
            CancellationToken cancellationToken = default)
        {
            var email = NormalizarEmail(request.Email);
            var nroMatricula = request.NroMatricula.Trim();

            // El email es único entre todos los usuarios (los profesionales también lo son).
            if (await _usuarioRepository.ExisteEmailAsync(email, cancellationToken))
            {
                throw new ConflictoException(
                    $"Ya existe un usuario registrado con el email '{email}'.");
            }

            if (await _profesionalRepository.ExisteMatriculaAsync(nroMatricula, cancellationToken))
            {
                throw new ConflictoException(
                    $"Ya existe un profesional registrado con la matrícula '{nroMatricula}'.");
            }

            var profesional = new Profesional(
                request.Nombre.Trim(),
                request.Apellido.Trim(),
                email,
                _hasherContrasenia.Hashear(request.Contrasenia),
                request.Telefono.Trim(),
                request.Direccion.Trim(),
                request.Zona.Trim(),
                request.TipoProfesional.Trim(),
                nroMatricula);

            await _profesionalRepository.AgregarAsync(profesional, cancellationToken);
            await _profesionalRepository.GuardarCambiosAsync(cancellationToken);

            return ProfesionalResponse.DesdeEntidad(profesional);
        }

        public async Task<ProfesionalResponse?> ActualizarAsync(
            int id,
            ActualizarProfesionalRequest request,
            CancellationToken cancellationToken = default)
        {
            var profesional = await _profesionalRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            if (profesional is null)
            {
                return null;
            }

            profesional.ActualizarPerfil(
                request.Nombre.Trim(),
                request.Apellido.Trim(),
                request.Telefono.Trim(),
                request.Direccion.Trim());

            profesional.ActualizarDatosProfesionales(
                request.Zona.Trim(),
                request.TipoProfesional.Trim());

            await _profesionalRepository.GuardarCambiosAsync(cancellationToken);

            return ProfesionalResponse.DesdeEntidad(profesional);
        }

        public async Task<ProfesionalResponse?> ActualizarPresentacionAsync(
            int id,
            ActualizarPresentacionRequest request,
            CancellationToken cancellationToken = default)
        {
            var profesional = await _profesionalRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            if (profesional is null)
            {
                return null;
            }

            profesional.ActualizarPresentacion(
                request.FotoPerfil?.Trim() ?? string.Empty,
                request.ComentarioPresentacion?.Trim() ?? string.Empty);

            await _profesionalRepository.GuardarCambiosAsync(cancellationToken);

            return ProfesionalResponse.DesdeEntidad(profesional);
        }

        public async Task<ProfesionalResponse?> CambiarDisponibilidadAsync(
            int id,
            CambiarDisponibilidadRequest request,
            CancellationToken cancellationToken = default)
        {
            var profesional = await _profesionalRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            if (profesional is null)
            {
                return null;
            }

            if (profesional.Estado == EstadoProfesionalEnum.DadoDeBaja)
            {
                throw new ConflictoException(
                    "Un profesional dado de baja no puede cambiar su disponibilidad.");
            }

            profesional.CambiarDisponibilidad(request.Disponible!.Value);
            await _profesionalRepository.GuardarCambiosAsync(cancellationToken);

            return ProfesionalResponse.DesdeEntidad(profesional);
        }

        public async Task<bool> DarDeBajaAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var profesional = await _profesionalRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            if (profesional is null)
            {
                return false;
            }

            profesional.DarDeBaja();
            await _profesionalRepository.GuardarCambiosAsync(cancellationToken);

            return true;
        }

        private static string NormalizarEmail(string email)
            => email.Trim().ToLowerInvariant();
    }
}