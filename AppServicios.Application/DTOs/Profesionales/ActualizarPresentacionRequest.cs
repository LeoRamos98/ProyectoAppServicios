using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Profesionales
{
    public class ActualizarPresentacionRequest
    {
        [MaxLength(500)]
        public string? FotoPerfil { get; set; }

        [MaxLength(1000)]
        public string? ComentarioPresentacion { get; set; }
    }
}