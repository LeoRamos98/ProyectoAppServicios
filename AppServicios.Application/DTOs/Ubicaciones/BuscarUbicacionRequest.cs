using System.ComponentModel.DataAnnotations;

namespace AppServicios.Application.DTOs.Ubicaciones
{
    public class BuscarUbicacionRequest : IValidatableObject
    {
        public string? Direccion { get; set; }

        [Range(-90d, 90d)]
        public double? Latitud { get; set; }

        [Range(-180d, 180d)]
        public double? Longitud { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Direccion))
            {
                if (Latitud is null || Longitud is null)
                {
                    yield return new ValidationResult(
                        "Ingresá una dirección o las dos coordenadas del dispositivo.",
                        [nameof(Direccion), nameof(Latitud), nameof(Longitud)]);
                }
            }
            else if (Latitud.HasValue || Longitud.HasValue)
            {
                yield return new ValidationResult(
                    "Elegí buscar por dirección o por coordenadas.",
                    [nameof(Direccion), nameof(Latitud), nameof(Longitud)]);
            }
        }
    }
}
