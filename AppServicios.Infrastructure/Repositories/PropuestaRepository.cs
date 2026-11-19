using AppServicios.Domain.Entities;
using AppServicios.Domain.Interfaces;
using AppServicios.Infrastructure.Data;
using AppServicios.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AppServicios.Infrastructure.Repositories
{
    public class PropuestaRepository : IPropuestaRepository
    {
        private readonly AppDbContext _context;

        public PropuestaRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Propuesta?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken)
        {
            return _context.Propuestas.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task AgregarAsync(
            Propuesta propuesta,
            CancellationToken cancellationToken = default)
        {
            await _context.Propuestas.AddAsync(propuesta, cancellationToken);
        }

        public Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task<List<Propuesta>> ObtenerPorUsuarioAsync(
             int idUsuario,
            CancellationToken cancellationToken = default)
            {
                return _context.Propuestas
                    .AsNoTracking()
                    .Where(propuesta => propuesta.IdUsuario == idUsuario)
                    .OrderBy(propuesta => propuesta.Id)
                    .ToListAsync(cancellationToken);
            }
        public Task<List<Propuesta>> ObtenerActivasAsync(
            CancellationToken cancellationToken = default)
            {
                return _context.Propuestas
                    .AsNoTracking()
                    .Where(propuesta => propuesta.Estado == EstadoPropuestaEnum.Activa)
                    .OrderBy(propuesta => propuesta.Id)
                    .ToListAsync(cancellationToken);
             }
    }

}