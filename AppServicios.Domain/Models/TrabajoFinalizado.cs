namespace AppServicios.Domain.Models;

public record TrabajoFinalizado(
    int IdEncargo,
    string TipoServicio,
    DateTime? FechaFinalizacion,
    int? Puntaje,
    string? Comentario)
{
    public string EstadoCalificacion => Puntaje.HasValue ? "Calificado" : "Sin calificación";
}
