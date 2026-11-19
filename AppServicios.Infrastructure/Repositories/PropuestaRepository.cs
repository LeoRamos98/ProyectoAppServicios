using AppServicios.Domain.Entities;
using AppServicios.Domain.Interfaces;
using AppServicios.Infrastructure.Data;

namespace AppServicios.Infrastructure.Repositories
{
    public class PropuestaRepository : IPropuestaRepository
    {
        private readonly AppDbContext _context;

        public PropuestaRepository(AppDbContext context)
        {
            _context = context;
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
    }
}