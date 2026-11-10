using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers
{
    [ApiController]
    [Route("api/encargos")]
    public class EncargosController : ControllerBase
    {
        private readonly IEncargoService _encargoService;

        public EncargosController(IEncargoService encargoService)
        {
            _encargoService = encargoService;
        }
    }
}
