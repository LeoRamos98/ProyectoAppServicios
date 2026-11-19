using AppServicios.Domain.Models;

namespace AppServicios.Application.DTOs.Profesionales;

public record PerfilProfesionalResponse(
    ProfesionalResponse Profesional,
    IReadOnlyList<TrabajoFinalizado> TrabajosRealizados,
    double? Promedio,
    int CantidadCalificaciones)
{
    public string EstadoCalificacion => Promedio.HasValue ? "Calificado" : "Sin calificación";
}
