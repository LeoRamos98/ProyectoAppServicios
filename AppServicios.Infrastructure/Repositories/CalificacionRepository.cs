using AppServicios.Domain.Entities;
using AppServicios.Domain.Interfaces;
using AppServicios.Domain.Models;
using AppServicios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppServicios.Infrastructure.Repositories
{
    public class CalificacionRepository : ICalificacionRepository
    {
        private readonly AppDbContext _context;

        public CalificacionRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Calificacion?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return _context.Calificaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public Task<Calificacion?> ObtenerPorEncargoAsync(
            int idEncargo,
            CancellationToken cancellationToken = default)
        {
            return _context.Calificaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdEncargo == idEncargo, cancellationToken);
        }

        public Task<List<Calificacion>> ObtenerPorProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default)
        {
            return _context.Calificaciones
                .AsNoTracking()
                .Where(c => c.IdProfesional == idProfesional)
                .OrderByDescending(c => c.FechaCalificacion)
                .ToListAsync(cancellationToken);
        }

        public async Task<ResumenCalificaciones> ObtenerResumenProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default)
        {
            // Se agrupa por profesional para resolver promedio y cantidad
            // en una sola consulta. Si no hay calificaciones no hay grupo.
            var resumen = await _context.Calificaciones
                .AsNoTracking()
                .Where(c => c.IdProfesional == idProfesional)
                .GroupBy(c => c.IdProfesional)
                .Select(g => new
                {
                    Promedio = g.Average(c => (double)c.Puntaje),
                    Cantidad = g.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return resumen is null
                ? new ResumenCalificaciones(null, 0)
                : new ResumenCalificaciones(
                    Math.Round(resumen.Promedio, 2),
                    resumen.Cantidad);
        }

        public Task<bool> ExisteParaEncargoAsync(
            int idEncargo,
            CancellationToken cancellationToken = default)
        {
            return _context.Calificaciones
                .AnyAsync(c => c.IdEncargo == idEncargo, cancellationToken);
        }

        public Task<bool> ExisteProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default)
        {
            return _context.Profesionales
                .AnyAsync(p => p.Id == idProfesional, cancellationToken);
        }

        public async Task AgregarAsync(
            Calificacion calificacion,
            CancellationToken cancellationToken = default)
        {
            await _context.Calificaciones.AddAsync(calificacion, cancellationToken);
        }

        public Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
