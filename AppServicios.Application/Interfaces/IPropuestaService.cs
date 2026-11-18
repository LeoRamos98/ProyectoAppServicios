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
    }
}