using AppServicios.Application.DTOs.Presupuestos;

namespace AppServicios.Application.Interfaces;

public interface IProfesionalPropuestaService
{
    Task<PresupuestoResponse> CrearAsync(int usuario, CrearPresupuestoRequest request, CancellationToken ct);
    Task<IReadOnlyList<PresupuestoResponse>> RecibidosAsync(int usuario, int propuesta, CancellationToken ct);
    Task<IReadOnlyList<PresupuestoResponse>> MiosAsync(int usuario, CancellationToken ct);
    Task<PresupuestoResponse> AceptarAsync(int usuario, int presupuesto, CancellationToken ct);
    Task<ConfirmacionPresupuestoResponse> ConfirmarAsync(int usuario, int presupuesto, CancellationToken ct);
    Task<PresupuestoResponse> RetirarAsync(int usuario, int presupuesto, CancellationToken ct);
}
