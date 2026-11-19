using AppServicios.Domain.Entities;

namespace AppServicios.Domain.Interfaces;

public interface IProfesionalPropuestaRepository
{
    Task<ProfesionalPropuesta?> ObtenerAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExisteOfertaAsync(int idProfesional, int idPropuesta, CancellationToken cancellationToken);
    void Agregar(ProfesionalPropuesta presupuesto);
    Task<List<ProfesionalPropuesta>> ObtenerDePropuestaAsync(int idPropuesta, CancellationToken cancellationToken);
    Task<List<ProfesionalPropuesta>> ObtenerDeProfesionalAsync(int idProfesional, CancellationToken cancellationToken);
    Task<bool> TieneEncargoActivoAsync(int idProfesional, CancellationToken cancellationToken);
    Task<bool> ExisteEncargoDePropuestaAsync(int idPropuesta, CancellationToken cancellationToken);
    Task<int?> ObtenerIdEncargoAsync(int idPresupuesto, CancellationToken cancellationToken);
    Task<int> CrearEncargoConChatAsync(Encargo encargo, int idUsuario, int idProfesional, CancellationToken cancellationToken);

    Task<List<ProfesionalPropuesta>> ObtenerPendientesAsync(
        int idPropuesta, CancellationToken cancellationToken);
}
