using AppServicios.Application.DTOs.Calificaciones;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class CalificacionService : ICalificacionService
    {
        private readonly ICalificacionRepository _calificacionRepository;
        private readonly IEncargoRepository _encargoRepository;

        public CalificacionService(
            ICalificacionRepository calificacionRepository,
            IEncargoRepository encargoRepository)
        {
            _calificacionRepository = calificacionRepository;
            _encargoRepository = encargoRepository;
        }

        public async Task<CalificacionResponse?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var calificacion = await _calificacionRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            return calificacion is null
                ? null
                : CalificacionResponse.DesdeEntidad(calificacion);
        }

        public async Task<CalificacionResponse?> ObtenerPorEncargoAsync(
            int idEncargo,
            CancellationToken cancellationToken = default)
        {
            var calificacion = await _calificacionRepository
                .ObtenerPorEncargoAsync(idEncargo, cancellationToken);

            return calificacion is null
                ? null
                : CalificacionResponse.DesdeEntidad(calificacion);
        }

        public async Task<CalificacionesProfesionalResponse?> ObtenerPorProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default)
        {
            if (!await _calificacionRepository
                    .ExisteProfesionalAsync(idProfesional, cancellationToken))
            {
                return null;
            }

            var resumen = await _calificacionRepository
                .ObtenerResumenProfesionalAsync(idProfesional, cancellationToken);

            var calificaciones = await _calificacionRepository
                .ObtenerPorProfesionalAsync(idProfesional, cancellationToken);

            return new CalificacionesProfesionalResponse(
                idProfesional,
                resumen.Promedio,
                resumen.Cantidad,
                calificaciones.Select(CalificacionResponse.DesdeEntidad).ToList());
        }

        public async Task<CalificacionResponse?> CalificarEncargoAsync(
            int idEncargo,
            CrearCalificacionRequest request,
            CancellationToken cancellationToken = default)
        {
            var detalle = await _encargoRepository
                .ObtenerPorIdAsync(idEncargo, cancellationToken);

            if (detalle is null)
            {
                return null;
            }

            // Solo califica el usuario que solicitó el servicio (dueño de la propuesta).
            if (request.IdUsuario != detalle.IdUsuario)
            {
                throw new AccesoDenegadoException(
                    "Solo el usuario que solicitó el encargo puede calificarlo.");
            }

            if (detalle.Encargo.Estado != EstadoEncargoEnum.Finalizado)
            {
                throw new ConflictoException(
                    "Solo se puede calificar un encargo finalizado.");
            }

            if (await _calificacionRepository
                    .ExisteParaEncargoAsync(idEncargo, cancellationToken))
            {
                throw new ConflictoException(
                    "El encargo ya fue calificado.");
            }

            Calificacion calificacion;
            try
            {
                calificacion = new Calificacion(
                    idEncargo,
                    detalle.IdProfesional,
                    detalle.IdUsuario,
                    request.Puntaje,
                    request.Comentario);
            }
            catch (ArgumentException ex)
            {
                // ArgumentOutOfRangeException también entra acá.
                throw new ConflictoException(ex.Message);
            }

            await _calificacionRepository.AgregarAsync(calificacion, cancellationToken);
            await _calificacionRepository.GuardarCambiosAsync(cancellationToken);

            return CalificacionResponse.DesdeEntidad(calificacion);
        }
    }
}
