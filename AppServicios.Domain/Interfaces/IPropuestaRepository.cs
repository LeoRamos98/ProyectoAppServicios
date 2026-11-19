using AppServicios.Domain.Entities;

namespace AppServicios.Domain.Interfaces
{
    public interface IPropuestaRepository
    {
        Task AgregarAsync(
            Propuesta propuesta,
            CancellationToken cancellationToken = default);

        Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default);

        Task<List<Propuesta>> ObtenerActivasAsync(
            CancellationToken cancellationToken = default);
    }
}