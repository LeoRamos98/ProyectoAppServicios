using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Route("api/presupuestos")]
    public class PresupuestosController : ControllerBase
    {
        private readonly IProfesionalPropuestaService
            _profesionalPropuestaService;

        public PresupuestosController(
            IProfesionalPropuestaService profesionalPropuestaService)
        {
            _profesionalPropuestaService = profesionalPropuestaService;
        }
    }
}