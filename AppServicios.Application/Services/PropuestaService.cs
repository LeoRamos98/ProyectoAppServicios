using System.ComponentModel.DataAnnotations;
using AppServicios.Application.DTOs.Propuestas;
using AppServicios.Application.Interfaces;
using AppServicios.Application.Exceptions;
using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class PropuestaService : IPropuestaService
    {
        private readonly IPropuestaRepository _propuestaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        private readonly IProfesionalPropuestaRepository _presupuestos;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;

        public PropuestaService(
            IPropuestaRepository propuestaRepository,
            IUsuarioRepository usuarioRepository,
            IProfesionalPropuestaRepository presupuestos,
            IUnidadDeTrabajo unidadDeTrabajo)
        {
            _propuestaRepository = propuestaRepository;
            _usuarioRepository = usuarioRepository;
            _presupuestos = presupuestos;
            _unidadDeTrabajo = unidadDeTrabajo;
        }

        public async Task<PropuestaResponse?> CrearAsync(
            int idUsuarioAutenticado,
            CrearPropuestaRequest request,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(
                idUsuarioAutenticado, cancellationToken);

            if (usuario is null || !usuario.Activo)
            {
                return null;
            }

            Validator.ValidateObject(
                request,
                new ValidationContext(request),
                validateAllProperties: true);

            var propuesta = new Propuesta(
                usuario.Id,
                request.TipoServicio.Trim(),
                request.Descripcion.Trim(),
                request.Ubicacion,
                request.FechaSolicitada!.Value,
                request.FotoPropuesta?.Trim());

            propuesta.ConfirmarUbicacion(
                request.Ubicacion,
                request.Latitud!.Value,
                request.Longitud!.Value);

            await _propuestaRepository.AgregarAsync(propuesta, cancellationToken);
            await _propuestaRepository.GuardarCambiosAsync(cancellationToken);

            return PropuestaResponse.DesdeEntidad(propuesta);
        }

        public Task<PropuestaResponse> ActualizarAsync(
            int idUsuario, int idPropuesta, CrearPropuestaRequest request, CancellationToken cancellationToken)
        {
            Validator.ValidateObject(request, new ValidationContext(request), validateAllProperties: true);
            return _unidadDeTrabajo.EjecutarAsync(async () =>
            {
                var propuesta = await ObtenerPropiaAsync(idUsuario, idPropuesta, cancellationToken);
                if (propuesta.Estado != EstadoPropuestaEnum.Activa)
                    throw new ConflictoException("Solo se puede editar una propuesta activa.");

                propuesta.Actualizar(request.TipoServicio, request.Descripcion, request.Ubicacion,
                    request.Latitud!.Value, request.Longitud!.Value, request.FechaSolicitada!.Value,
                    request.FotoPropuesta);
                return PropuestaResponse.DesdeEntidad(propuesta);
            }, cancellationToken);
        }

        public Task<PropuestaResponse> CancelarAsync(
            int idUsuario, int idPropuesta, CancellationToken cancellationToken)
        {
            return _unidadDeTrabajo.EjecutarAsync(async () =>
            {
                var propuesta = await ObtenerPropiaAsync(idUsuario, idPropuesta, cancellationToken);
                if (propuesta.Estado == EstadoPropuestaEnum.Adjudicada)
                    throw new ConflictoException("La propuesta ya fue adjudicada.");

                if (propuesta.Estado == EstadoPropuestaEnum.Activa)
                {
                    var pendientes = await _presupuestos.ObtenerPendientesAsync(idPropuesta, cancellationToken);
                    foreach (var presupuesto in pendientes)
                        presupuesto.Cancelar();

                    propuesta.Cancelar();
                }
                return PropuestaResponse.DesdeEntidad(propuesta);
            }, cancellationToken);
        }

        private async Task<Propuesta> ObtenerPropiaAsync(
            int idUsuario, int idPropuesta, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuario, cancellationToken);
            if (usuario is null || !usuario.Activo)
                throw new UnauthorizedAccessException("La cuenta debe estar activa.");

            var propuesta = await _propuestaRepository.ObtenerPorIdAsync(idPropuesta, cancellationToken)
                ?? throw new KeyNotFoundException("La propuesta no existe.");
            if (propuesta.IdUsuario != idUsuario)
                throw new UnauthorizedAccessException("Solo el propietario puede cambiar esta propuesta.");

            return propuesta;
        }

        public async Task<IReadOnlyList<PropuestaResponse>?> ObtenerMiasAsync(
            int idUsuarioAutenticado,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(
                idUsuarioAutenticado, cancellationToken);

            if (usuario is null || !usuario.Activo)
            {
                return null;
            }

            var propuestas = await _propuestaRepository.ObtenerPorUsuarioAsync(
                usuario.Id, cancellationToken);

            return propuestas.Select(PropuestaResponse.DesdeEntidad).ToList();
        }

        public async Task<IReadOnlyList<PropuestaResponse>?> ObtenerActivasAsync(
            int idUsuarioAutenticado,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(
                idUsuarioAutenticado, cancellationToken);

            if (usuario is not Profesional profesional ||
                !profesional.Activo ||
                profesional.Estado != EstadoProfesionalEnum.Disponible)
            {
                return null;
            }

            var propuestas = await _propuestaRepository.ObtenerActivasAsync(
                cancellationToken);

            return propuestas.Select(PropuestaResponse.DesdeEntidad).ToList();
        }
    }
}
