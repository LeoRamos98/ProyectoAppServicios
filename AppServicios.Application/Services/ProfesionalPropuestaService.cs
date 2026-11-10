using AppServicios.Application.Interfaces;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class ProfesionalPropuestaService
        : IProfesionalPropuestaService
    {
        private readonly IProfesionalPropuestaRepository
            _profesionalPropuestaRepository;

        public ProfesionalPropuestaService(
            IProfesionalPropuestaRepository profesionalPropuestaRepository)
        {
            _profesionalPropuestaRepository = profesionalPropuestaRepository;
        }
    }
}