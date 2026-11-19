using AppServicios.Domain.Entities;
using AppServicios.Domain.Interfaces;
using AppServicios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppServicios.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Usuario>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<Usuario?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public Task<Usuario?> ObtenerPorEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            return _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public Task<bool> ExisteEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            return _context.Usuarios
                .AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task AgregarAsync(
            Usuario usuario,
            CancellationToken cancellationToken = default)
        {
            await _context.Usuarios.AddAsync(usuario, cancellationToken);
        }

        public Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
