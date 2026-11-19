using AppServicios.Domain.Interfaces;
using AppServicios.Domain.Models;
using AppServicios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppServicios.Infrastructure.Repositories
{
    public class EncargoRepository : IEncargoRepository
    {

        private readonly AppDbContext _context;

        public EncargoRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<EncargoDetalle>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return (from e in _context.Encargos.AsNoTracking()
                    join pp in _context.Presupuestos
                        on e.IdProfesionalPropuesta equals pp.Id
                    join p in _context.Propuestas
                        on e.IdPropuesta equals p.Id
                    orderby e.Id
                    select new EncargoDetalle(e, p.IdUsuario, pp.IdProfesional))
                .ToListAsync(cancellationToken);
        }

        public Task<EncargoDetalle?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return (from e in _context.Encargos
                    join pp in _context.Presupuestos
                        on e.IdProfesionalPropuesta equals pp.Id
                    join p in _context.Propuestas
                        on e.IdPropuesta equals p.Id
                    where e.Id == id
                    select new EncargoDetalle(e, p.IdUsuario, pp.IdProfesional))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<EncargoDetalle>> ObtenerPorUsuarioAsync(
            int idUsuario,
            CancellationToken cancellationToken = default)
        {
            return (from e in _context.Encargos.AsNoTracking()
                    join pp in _context.Presupuestos
                        on e.IdProfesionalPropuesta equals pp.Id
                    join p in _context.Propuestas
                        on e.IdPropuesta equals p.Id
                    where p.IdUsuario == idUsuario
                    orderby e.Id
                    select new EncargoDetalle(e, p.IdUsuario, pp.IdProfesional))
                .ToListAsync(cancellationToken);
        }

        public Task<List<EncargoDetalle>> ObtenerPorProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default)
        {
            return (from e in _context.Encargos.AsNoTracking()
                    join pp in _context.Presupuestos
                        on e.IdProfesionalPropuesta equals pp.Id
                    join p in _context.Propuestas
                        on e.IdPropuesta equals p.Id
                    where pp.IdProfesional == idProfesional
                    orderby e.Id
                    select new EncargoDetalle(e, p.IdUsuario, pp.IdProfesional))
                .ToListAsync(cancellationToken);
        }

        public Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
