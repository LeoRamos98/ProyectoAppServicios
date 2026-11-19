using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Encargos
{
    /// <summary>
    /// Datos para iniciar, finalizar o cancelar un encargo.
    /// </summary>
    public class AccionEncargoRequest
    {
        // Temporal: cuando exista autenticación, este id se obtiene del
        // token y deja de viajar en el cuerpo de la solicitud.
        // El id de un Profesional es el mismo que el de su Usuario.
        [Range(1, int.MaxValue)]
        public int IdUsuario { get; set; }
    }
}