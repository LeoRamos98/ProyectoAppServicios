using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Autenticacion
{
    public class LoginRequest
    {
        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Contrasenia { get; set; } = string.Empty;
    }
}