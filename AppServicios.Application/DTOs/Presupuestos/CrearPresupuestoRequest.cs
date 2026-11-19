using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Presupuestos;

public class CrearPresupuestoRequest
{
    [Range(1, int.MaxValue)]
    public int IdPropuesta { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true)]
    public decimal Monto { get; set; }

    [Required]
    public string Descripcion { get; set; } = string.Empty;
}
