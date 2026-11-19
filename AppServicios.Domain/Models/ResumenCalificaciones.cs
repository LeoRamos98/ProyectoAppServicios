namespace AppServicios.Domain.Models
{
    /// <summary>
    /// Promedio y cantidad de calificaciones recibidas por un profesional.
    /// Promedio es null cuando todavía no tiene calificaciones.
    /// </summary>
    public record ResumenCalificaciones(
        double? Promedio,
        int Cantidad);
}
