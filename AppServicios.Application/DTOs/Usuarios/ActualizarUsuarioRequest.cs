using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Usuarios
{
    public class ActualizarUsuarioRequest
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required, Phone, MaxLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;
    }
}
