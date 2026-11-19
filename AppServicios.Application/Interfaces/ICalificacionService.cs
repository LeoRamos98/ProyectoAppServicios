using AppServicios.Application.DTOs.Calificaciones;

namespace AppServicios.Application.Interfaces
{
    public interface ICalificacionService
    {
        Task<CalificacionResponse?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Devuelve null si el encargo todavía no fue calificado.
        /// </summary>
        Task<CalificacionResponse?> ObtenerPorEncargoAsync(
            int idEncargo,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Devuelve null si el profesional no existe.
        /// </summary>
        Task<CalificacionesProfesionalResponse?> ObtenerPorProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Califica al profesional de un encargo. Devuelve null si el encargo no existe.
        /// </summary>
        /// <exception cref="Exceptions.AccesoDenegadoException">
        /// Si quien califica no es el usuario que solicitó el encargo.
        /// </exception>
        /// <exception cref="Exceptions.ConflictoException">
        /// Si el encargo no está finalizado o ya fue calificado.
        /// </exception>
        Task<CalificacionResponse?> CalificarEncargoAsync(
            int idEncargo,
            int idUsuarioAutenticado, CrearCalificacionRequest request,
            CancellationToken cancellationToken = default);
    }
}
