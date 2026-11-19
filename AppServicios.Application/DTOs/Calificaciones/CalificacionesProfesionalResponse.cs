namespace AppServicios.Application.DTOs.Calificaciones
{
    /// <summary>
    /// Reputación de un profesional: promedio, cantidad y detalle
    /// de las calificaciones recibidas (más recientes primero).
    /// </summary>
    public record CalificacionesProfesionalResponse(
        int IdProfesional,
        double? Promedio,
        int Cantidad,
        IReadOnlyList<CalificacionResponse> Calificaciones);
}
