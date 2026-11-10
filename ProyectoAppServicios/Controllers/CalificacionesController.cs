using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Route("api/calificaciones")]
    public class CalificacionesController : ControllerBase
    {
        private readonly ICalificacionService _calificacionService;

        public CalificacionesController(
            ICalificacionService calificacionService)
        {
            _calificacionService = calificacionService;
        }
    }
}
