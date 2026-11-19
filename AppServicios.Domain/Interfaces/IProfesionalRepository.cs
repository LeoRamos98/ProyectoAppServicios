using AppServicios.Domain.Entities;
using AppServicios.Domain.Models;

namespace AppServicios.Domain.Interfaces
{
    public interface IProfesionalRepository
    {
        Task<List<Profesional>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task<Profesional?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<TrabajoFinalizado>> ObtenerTrabajosFinalizadosAsync(
            int idProfesional, CancellationToken cancellationToken = default);

        Task<bool> ExisteMatriculaAsync(
            string nroMatricula,
            CancellationToken cancellationToken = default);

        Task AgregarAsync(
            Profesional profesional,
            CancellationToken cancellationToken = default);

        Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default);
    }
}