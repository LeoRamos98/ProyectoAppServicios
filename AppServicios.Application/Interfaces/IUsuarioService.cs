using AppServicios.Application.DTOs.Usuarios;

namespace AppServicios.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<IReadOnlyList<UsuarioResponse>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task<UsuarioResponse?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <exception cref="Exceptions.ConflictoException">
        /// Si el email ya está registrado.
        /// </exception>
        Task<UsuarioResponse> CrearAsync(
            CrearUsuarioRequest request,
            CancellationToken cancellationToken = default);

        /// <returns>El usuario actualizado, o null si no existe.</returns>
        Task<UsuarioResponse?> ActualizarAsync(
            int id,
            ActualizarUsuarioRequest request,
            CancellationToken cancellationToken = default);

        /// <returns>false si el usuario no existe.</returns>
        Task<bool> DesactivarAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
