using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;
using AppServicios.Domain.Interfaces;
using AppServicios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppServicios.Infrastructure.Repositories;

public class ProfesionalPropuestaRepository(AppDbContext contexto) : IProfesionalPropuestaRepository
{
    public Task<ProfesionalPropuesta?> ObtenerAsync(int id, CancellationToken cancellationToken) =>
        contexto.Presupuestos.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ExisteOfertaAsync(int idProfesional, int idPropuesta, CancellationToken cancellationToken) =>
        contexto.Presupuestos.AnyAsync(p => p.IdProfesional == idProfesional && p.IdPropuesta == idPropuesta, cancellationToken);

    public void Agregar(ProfesionalPropuesta presupuesto) => contexto.Presupuestos.Add(presupuesto);

    public Task<List<ProfesionalPropuesta>> ObtenerDePropuestaAsync(int idPropuesta, CancellationToken cancellationToken) =>
        contexto.Presupuestos.AsNoTracking().Where(p => p.IdPropuesta == idPropuesta)
            .OrderBy(p => p.Id).ToListAsync(cancellationToken);

    public Task<List<ProfesionalPropuesta>> ObtenerDeProfesionalAsync(int idProfesional, CancellationToken cancellationToken) =>
        contexto.Presupuestos.AsNoTracking().Where(p => p.IdProfesional == idProfesional)
            .OrderBy(p => p.Id).ToListAsync(cancellationToken);

    public Task<bool> TieneEncargoActivoAsync(int idProfesional, CancellationToken cancellationToken) =>
        (from e in contexto.Encargos
         join p in contexto.Presupuestos on e.IdProfesionalPropuesta equals p.Id
         where p.IdProfesional == idProfesional &&
               (e.Estado == EstadoEncargoEnum.PendienteDeInicio || e.Estado == EstadoEncargoEnum.EnCurso)
         select e.Id).AnyAsync(cancellationToken);

    public Task<bool> ExisteEncargoDePropuestaAsync(int idPropuesta, CancellationToken cancellationToken) =>
        contexto.Encargos.AnyAsync(e => e.IdPropuesta == idPropuesta, cancellationToken);

    public Task<int?> ObtenerIdEncargoAsync(int idPresupuesto, CancellationToken cancellationToken) =>
        contexto.Encargos.Where(e => e.IdProfesionalPropuesta == idPresupuesto)
            .Select(e => (int?)e.Id).SingleOrDefaultAsync(cancellationToken);

    public async Task<int> CrearEncargoConChatAsync(
        Encargo encargo, int idUsuario, int idProfesional, CancellationToken cancellationToken)
    {
        contexto.Encargos.Add(encargo);
        // SQL asigna el Id dentro de la transacción, antes de crear el chat.
        await contexto.SaveChangesAsync(cancellationToken);
        contexto.Chats.Add(new Chat(encargo.Id, idUsuario, idProfesional));
        return encargo.Id;
    }

    public Task<List<ProfesionalPropuesta>> ObtenerPendientesAsync(
        int idPropuesta, CancellationToken cancellationToken)
    {
        return contexto.Presupuestos
            .Where(p => p.IdPropuesta == idPropuesta && p.Estado == EstadoPresupuestoEnum.Pendiente)
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }
}
