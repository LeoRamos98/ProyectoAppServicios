using AppServicios.Application.DTOs.Ubicaciones;

namespace AppServicios.Application.Interfaces
{
    public interface IUbicacionService
    {
        Task<IReadOnlyList<UbicacionResponse>> BuscarPorDireccionAsync(
            string direccion, CancellationToken cancellationToken);

        Task<IReadOnlyList<UbicacionResponse>> BuscarPorCoordenadasAsync(
            double latitud, double longitud, CancellationToken cancellationToken);
    }
}
