using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Route("api/propuestas")]
    public class PropuestasController : ControllerBase
    {
        private readonly IPropuestaService _propuestaService;

        public PropuestasController(IPropuestaService propuestaService)
        {
            _propuestaService = propuestaService;
        }
    }
}