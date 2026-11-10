using AppServicios.Application.Interfaces;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
    }
}