using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;

namespace AppServicios.Application.DTOs.Propuestas
{
    public record PropuestaResponse(
        int Id,
        int IdUsuario,
        string TipoServicio,
        string Descripcion,
        string Ubicacion,
        double? Latitud,
        double? Longitud,
        DateTime FechaSolicitada,
        string FotoPropuesta,
        EstadoPropuestaEnum Estado)
    {
        public static PropuestaResponse DesdeEntidad(Propuesta propuesta) => new(
            propuesta.Id,
            propuesta.IdUsuario,
            propuesta.TipoServicio,
            propuesta.Descripcion,
            propuesta.Ubicacion,
            propuesta.Latitud,
            propuesta.Longitud,
            propuesta.FechaSolicitada,
            propuesta.FotoPropuesta,
            propuesta.Estado);
    }
}