using AppServicios.Domain.Entities;
using AppServicios.Domain.Models;

namespace AppServicios.Domain.Interfaces
{
    public interface ICalificacionRepository
    {
        Task<Calificacion?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Calificacion?> ObtenerPorEncargoAsync(
            int idEncargo,
            CancellationToken cancellationToken = default);

        Task<List<Calificacion>> ObtenerPorProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default);

        Task<ResumenCalificaciones> ObtenerResumenProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteParaEncargoAsync(
            int idEncargo,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default);

        Task AgregarAsync(
            Calificacion calificacion,
            CancellationToken cancellationToken = default);

        Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default);
    }
}
