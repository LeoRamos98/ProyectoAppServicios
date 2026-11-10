using AppServicios.Application.Interfaces;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class MensajeService : IMensajeService
    {
        private readonly IMensajeRepository _mensajeRepository;

        public MensajeService(IMensajeRepository mensajeRepository)
        {
            _mensajeRepository = mensajeRepository;
        }
    }
}