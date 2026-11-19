using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Profesionales
{
    public class CambiarDisponibilidadRequest
    {
        /// <summary>
        /// true = Disponible, false = Ocupado.
        /// </summary>
        [Required]
        public bool? Disponible { get; set; }
    }
}