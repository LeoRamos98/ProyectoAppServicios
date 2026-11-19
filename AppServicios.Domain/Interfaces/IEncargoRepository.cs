using AppServicios.Domain.Entities;
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

        Task<bool> ExisteParaPropuestaAsync(
            int idPropuesta,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lectura del presupuesto a partir del cual se genera el encargo.
        /// </summary>
        Task<ProfesionalPropuesta?> ObtenerPresupuestoAsync(
            int idProfesionalPropuesta,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lectura de la propuesta a la que pertenece el presupuesto.
        /// </summary>
        Task<Propuesta?> ObtenerPropuestaAsync(
            int idPropuesta,
            CancellationToken cancellationToken = default);

        Task AgregarAsync(
            Encargo encargo,
            CancellationToken cancellationToken = default);

        Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default);
    }
}