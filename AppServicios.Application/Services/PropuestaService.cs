using AppServicios.Application.Interfaces;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class PropuestaService : IPropuestaService
    {
        private readonly IPropuestaRepository _propuestaRepository;

        public PropuestaService(IPropuestaRepository propuestaRepository)
        {
            _propuestaRepository = propuestaRepository;
        }
    }
}