using AppServicios.Application.DTOs.Profesionales;

namespace AppServicios.Application.Interfaces
{
    public interface IProfesionalService
    {
        Task<IReadOnlyList<ProfesionalResponse>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task<ProfesionalResponse?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <exception cref="Exceptions.ConflictoException">
        /// Si el email o el número de matrícula ya están registrados.
        /// </exception>
        Task<ProfesionalResponse> CrearAsync(
            CrearProfesionalRequest request,
            CancellationToken cancellationToken = default);

        /// <returns>El profesional actualizado, o null si no existe.</returns>
        Task<ProfesionalResponse?> ActualizarAsync(
            int id,
            ActualizarProfesionalRequest request,
            CancellationToken cancellationToken = default);

        /// <returns>El profesional actualizado, o null si no existe.</returns>
        Task<ProfesionalResponse?> ActualizarPresentacionAsync(
            int id,
            ActualizarPresentacionRequest request,
            CancellationToken cancellationToken = default);

        /// <returns>El profesional actualizado, o null si no existe.</returns>
        /// <exception cref="Exceptions.ConflictoException">
        /// Si el profesional está dado de baja.
        /// </exception>
        Task<ProfesionalResponse?> CambiarDisponibilidadAsync(
            int id,
            CambiarDisponibilidadRequest request,
            CancellationToken cancellationToken = default);

        /// <returns>false si el profesional no existe.</returns>
        Task<bool> DarDeBajaAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}