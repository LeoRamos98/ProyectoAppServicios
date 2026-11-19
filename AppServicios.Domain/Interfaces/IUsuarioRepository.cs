using AppServicios.Domain.Enums;
using AppServicios.Domain.Entities;

namespace AppServicios.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<RolEnum?> ObtenerRolActivoAsync(int id, CancellationToken cancellationToken = default);

        Task<List<Usuario>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task<Usuario?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);
        Task<Usuario?> ObtenerPorEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task AgregarAsync(
            Usuario usuario,
            CancellationToken cancellationToken = default);

        Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default);
    }
}
