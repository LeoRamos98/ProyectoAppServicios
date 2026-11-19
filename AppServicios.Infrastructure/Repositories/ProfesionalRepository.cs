using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;
using AppServicios.Domain.Models;
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

        public Task<List<TrabajoFinalizado>> ObtenerTrabajosFinalizadosAsync(
            int idProfesional, CancellationToken cancellationToken = default)
        {
            return (from e in _context.Encargos.AsNoTracking()
                    join oferta in _context.Presupuestos on e.IdProfesionalPropuesta equals oferta.Id
                    join propuesta in _context.Propuestas on e.IdPropuesta equals propuesta.Id
                    join nota in _context.Calificaciones on e.Id equals nota.IdEncargo into notas
                    from nota in notas.DefaultIfEmpty()
                    where oferta.IdProfesional == idProfesional && e.Estado == EstadoEncargoEnum.Finalizado
                    orderby e.FechaFinalizacionEncargo descending, e.Id descending
                    select new TrabajoFinalizado(e.Id, propuesta.TipoServicio, e.FechaFinalizacionEncargo,
                        nota == null ? null : (int?)nota.Puntaje, nota == null ? null : nota.Comentario))
                .ToListAsync(cancellationToken);
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