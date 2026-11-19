using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Propuestas
{
    public class CrearPropuestaRequest
    {
        [Required]
        public string TipoServicio { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public string Ubicacion { get; set; } = string.Empty;

        [Required, Range(-90d, 90d)]
        public double? Latitud { get; set; }

        [Required, Range(-180d, 180d)]
        public double? Longitud { get; set; }

        [Required]
        public DateTime? FechaSolicitada { get; set; }

        public string? FotoPropuesta { get; set; }
    }
}