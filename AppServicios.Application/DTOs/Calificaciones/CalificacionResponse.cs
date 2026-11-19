using AppServicios.Domain.Entities;

namespace AppServicios.Application.DTOs.Calificaciones
{
    public record CalificacionResponse(
        int Id,
        int IdEncargo,
        int IdProfesional,
        int IdUsuario,
        int Puntaje,
        string Comentario,
        DateTime FechaCalificacion)
    {
        public static CalificacionResponse DesdeEntidad(Calificacion calificacion) => new(
            calificacion.Id,
            calificacion.IdEncargo,
            calificacion.IdProfesional,
            calificacion.IdUsuario,
            calificacion.Puntaje,
            calificacion.Comentario,
            calificacion.FechaCalificacion);
    }
}
