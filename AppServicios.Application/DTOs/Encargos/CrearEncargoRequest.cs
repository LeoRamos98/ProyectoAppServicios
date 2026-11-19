using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Encargos
{
    public class CrearEncargoRequest
    {
        [Range(1, int.MaxValue)]
        public int IdProfesionalPropuesta { get; set; }
    }
}