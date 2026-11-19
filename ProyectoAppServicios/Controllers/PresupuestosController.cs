using AppServicios.Application.DTOs.Presupuestos;
using AppServicios.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers;

[ApiController]
[Authorize]
[Route("api/presupuestos")]
public class PresupuestosController(IProfesionalPropuestaService servicio) : ControladorAutenticado
{
    [HttpPost]
    public Task<ActionResult<PresupuestoResponse>> Crear([FromBody] CrearPresupuestoRequest request, CancellationToken ct) =>
        EjecutarAsync(usuario => servicio.CrearAsync(usuario, request, ct), StatusCodes.Status201Created);

    [HttpGet("propuesta/{idPropuesta:int}")]
    public Task<ActionResult<IReadOnlyList<PresupuestoResponse>>> Recibidos(int idPropuesta, CancellationToken ct) =>
        EjecutarAsync(usuario => servicio.RecibidosAsync(usuario, idPropuesta, ct));

    [HttpGet("mios")]
    public Task<ActionResult<IReadOnlyList<PresupuestoResponse>>> Mios(CancellationToken ct) =>
        EjecutarAsync(usuario => servicio.MiosAsync(usuario, ct));

    [HttpPost("{id:int}/aceptar")]
    public Task<ActionResult<PresupuestoResponse>> Aceptar(int id, CancellationToken ct) =>
        EjecutarAsync(usuario => servicio.AceptarAsync(usuario, id, ct));

    [HttpPost("{id:int}/confirmar")]
    public Task<ActionResult<ConfirmacionPresupuestoResponse>> Confirmar(int id, CancellationToken ct) =>
        EjecutarAsync(usuario => servicio.ConfirmarAsync(usuario, id, ct));

    [HttpDelete("{id:int}")]
    public Task<ActionResult<PresupuestoResponse>> Retirar(int id, CancellationToken ct) =>
        EjecutarAsync(usuario => servicio.RetirarAsync(usuario, id, ct));
}
