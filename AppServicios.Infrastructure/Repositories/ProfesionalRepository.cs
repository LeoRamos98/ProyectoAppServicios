using AppServicios.Domain.Entities;
using AppServicios.Domain.Interfaces;
using AppServicios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppServicios.Infrastructure.Repositories
{
    public class ProfesionalRepository : IProfesionalRepository
    {
        private readonly AppDbContext _context;

        public ProfesionalRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Profesional>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.Profesionales
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<Profesional?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return _context.Profesionales
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public Task<bool> ExisteMatriculaAsync(
            string nroMatricula,
            CancellationToken cancellationToken = default)
        {
            return _context.Profesionales
                .AnyAsync(p => p.NroMatricula == nroMatricula, cancellationToken);
        }

        public async Task AgregarAsync(
            Profesional profesional,
            CancellationToken cancellationToken = default)
        {
            await _context.Profesionales.AddAsync(profesional, cancellationToken);
        }

        public Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}