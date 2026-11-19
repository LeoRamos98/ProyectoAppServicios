using AppServicios.Application.DTOs.Propuestas;

namespace AppServicios.Application.Interfaces
{
    public interface IPropuestaService
    {
        /// <returns>La propuesta guardada, o null si el usuario no existe o está inactivo.</returns>
        Task<PropuestaResponse?> CrearAsync(
            int idUsuarioAutenticado,
            CrearPropuestaRequest request,
            CancellationToken cancellationToken = default);
        Task<PropuestaResponse> ActualizarAsync(
            int idUsuario, int idPropuesta, CrearPropuestaRequest request, CancellationToken cancellationToken);

        Task<PropuestaResponse> CancelarAsync(
            int idUsuario, int idPropuesta, CancellationToken cancellationToken);

        Task<IReadOnlyList<PropuestaResponse>?> ObtenerMiasAsync(
            int idUsuarioAutenticado,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PropuestaResponse>?> ObtenerActivasAsync(
            int idUsuarioAutenticado,
            CancellationToken cancellationToken = default);
    }
}