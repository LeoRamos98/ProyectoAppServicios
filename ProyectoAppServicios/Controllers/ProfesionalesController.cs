using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Route("api/profesionales")]
    public class ProfesionalesController : ControllerBase
    {
        private readonly IProfesionalService _profesionalService;

        public ProfesionalesController(
            IProfesionalService profesionalService)
        {
            _profesionalService = profesionalService;
        }
    }
}