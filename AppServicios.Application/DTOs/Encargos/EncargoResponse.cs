using AppServicios.Domain.Enums;
using AppServicios.Domain.Models;

namespace AppServicios.Application.DTOs.Encargos
{
    public record EncargoResponse(
        int Id,
        int IdPropuesta,
        int IdProfesionalPropuesta,
        int IdUsuario,
        int IdProfesional,
        string Ubicacion,
        EstadoEncargoEnum Estado,
        DateTime? FechaInicioEncargo,
        DateTime? FechaFinalizacionEncargo)
    {
        public static EncargoResponse DesdeDetalle(EncargoDetalle detalle) => new(
            detalle.Encargo.Id,
            detalle.Encargo.IdPropuesta,
            detalle.Encargo.IdProfesionalPropuesta,
            detalle.IdUsuario,
            detalle.IdProfesional,
            detalle.Encargo.Ubicacion,
            detalle.Encargo.Estado,
            detalle.Encargo.FechaInicioEncargo,
            detalle.Encargo.FechaFinalizacionEncargo);
    }
}