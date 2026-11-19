using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Calificaciones
{
    /// <summary>
    /// Datos para calificar al profesional de un encargo finalizado.
    /// El encargo viaja en la ruta (api/encargos/{idEncargo}/calificacion).
    /// </summary>
    public class CrearCalificacionRequest
    {
        // Temporal: cuando exista autenticación, este id se obtiene del
        // token y deja de viajar en el cuerpo de la solicitud.
        [Range(1, int.MaxValue)]
        public int IdUsuario { get; set; }

        [Range(1, 5, ErrorMessage = "El puntaje debe estar entre 1 y 5.")]
        public int Puntaje { get; set; }

        [MaxLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
        public string? Comentario { get; set; }
    }
}
