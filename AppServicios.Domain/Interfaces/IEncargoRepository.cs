using AppServicios.Domain.Models;

namespace AppServicios.Domain.Interfaces
{
    public interface IEncargoRepository
    {
        Task<List<EncargoDetalle>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task<EncargoDetalle?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<EncargoDetalle>> ObtenerPorUsuarioAsync(
            int idUsuario,
            CancellationToken cancellationToken = default);

        Task<List<EncargoDetalle>> ObtenerPorProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default);

        Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default);
    }
}