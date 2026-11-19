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
        private readonly IProfesionalPropuestaRepository _presupuestos;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;

        public ProfesionalService(
            IProfesionalRepository profesionalRepository,
            IUsuarioRepository usuarioRepository,
            IHasherContrasenia hasherContrasenia,
            IProfesionalPropuestaRepository presupuestos,
            IUnidadDeTrabajo unidadDeTrabajo)
        {
            _profesionalRepository = profesionalRepository;
            _usuarioRepository = usuarioRepository;
            _hasherContrasenia = hasherContrasenia;
            _presupuestos = presupuestos;
            _unidadDeTrabajo = unidadDeTrabajo;
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

        public async Task<PerfilProfesionalResponse?> ObtenerPerfilAsync(
            int id, CancellationToken cancellationToken = default)
        {
            var profesional = await ObtenerPorIdAsync(id, cancellationToken);
            if (profesional is null)
                return null;

            var trabajos = await _profesionalRepository.ObtenerTrabajosFinalizadosAsync(id, cancellationToken);
            var puntajes = trabajos.Where(t => t.Puntaje.HasValue).Select(t => t.Puntaje!.Value).ToList();
            return new PerfilProfesionalResponse(profesional, trabajos,
                puntajes.Count == 0 ? null : Math.Round(puntajes.Average(), 2), puntajes.Count);
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

        public Task<ProfesionalResponse?> CambiarDisponibilidadAsync(
            int id, CambiarDisponibilidadRequest request, CancellationToken cancellationToken = default)
        {
            return _unidadDeTrabajo.EjecutarAsync<ProfesionalResponse?>(async () =>
            {
                var profesional = await _profesionalRepository.ObtenerPorIdAsync(id, cancellationToken);
                if (profesional is null)
                    return null;
                if (!profesional.Activo || profesional.Estado == EstadoProfesionalEnum.DadoDeBaja)
                    throw new ConflictoException("Un profesional dado de baja no puede cambiar su disponibilidad.");

                bool disponible = request.Disponible!.Value;
                if (disponible && await _presupuestos.TieneEncargoActivoAsync(id, cancellationToken))
                    throw new ConflictoException("El profesional tiene un encargo activo.");

                profesional.CambiarDisponibilidad(disponible);
                return ProfesionalResponse.DesdeEntidad(profesional);
            }, cancellationToken);
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