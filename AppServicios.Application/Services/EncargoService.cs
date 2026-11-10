using AppServicios.Application.Interfaces;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class EncargoService : IEncargoService
    {
        private readonly IEncargoRepository _encargoRepository;

        public EncargoService(IEncargoRepository encargoRepository)
        {
            _encargoRepository = encargoRepository;
        }
    }
}