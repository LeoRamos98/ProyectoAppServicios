using AppServicios.Application.Interfaces;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class ProfesionalService : IProfesionalService
    {
        private readonly IProfesionalRepository _profesionalRepository;

        public ProfesionalService(
            IProfesionalRepository profesionalRepository)
        {
            _profesionalRepository = profesionalRepository;
        }
    }
}