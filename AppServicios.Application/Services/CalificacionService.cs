using AppServicios.Application.Interfaces;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class CalificacionService : ICalificacionService
    {
        private readonly ICalificacionRepository _calificacionRepository;

        public CalificacionService(
            ICalificacionRepository calificacionRepository)
        {
            _calificacionRepository = calificacionRepository;
        }
    }
}