using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;

namespace AppServicios.Application.DTOs.Presupuestos;

public record PresupuestoResponse(
    int Id, int IdPropuesta, int IdProfesional, decimal Monto, string Descripcion,
    DateTime FechaPresupuesto, EstadoPresupuestoEnum Estado, bool AprobadoUsuario, bool AprobadoProfesional)
{
    public static PresupuestoResponse DesdeEntidad(ProfesionalPropuesta p) =>
        new(p.Id, p.IdPropuesta, p.IdProfesional, p.Monto, p.Descripcion,
            p.FechaPresupuesto, p.Estado, p.AprobadoUsuario, p.AprobadoProfesional);
}

public record ConfirmacionPresupuestoResponse(PresupuestoResponse Presupuesto, int IdEncargo)
{
    internal bool EncargoCreado { get; init; }
}
