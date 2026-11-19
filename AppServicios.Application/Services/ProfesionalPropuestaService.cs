using System.ComponentModel.DataAnnotations;
using AppServicios.Application.DTOs.Presupuestos;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services;

public class ProfesionalPropuestaService(
    IProfesionalPropuestaRepository presupuestos,
    IPropuestaRepository propuestas,
    IUsuarioRepository usuarios,
    IUnidadDeTrabajo unidadDeTrabajo) : IProfesionalPropuestaService
{
    public async Task<PresupuestoResponse> CrearAsync(int usuario, CrearPresupuestoRequest request, CancellationToken ct)
    {
        Validator.ValidateObject(request, new ValidationContext(request), validateAllProperties: true);
        if (request.Monto != decimal.Round(request.Monto, 2))
            throw new ValidationException("El monto admite hasta dos decimales.");

        var creado = await unidadDeTrabajo.EjecutarAsync(async () =>
        {
            var profesional = await ProfesionalActivoAsync(usuario, ct);
            var propuesta = await BuscarPropuestaAsync(request.IdPropuesta, ct);
            if (propuesta.IdUsuario == usuario)
                throw new UnauthorizedAccessException("El profesional no puede ofertar por su propia propuesta.");
            ExigirActiva(propuesta);
            await ExigirDisponibilidadAsync(profesional, ct);
            if (await presupuestos.ExisteOfertaAsync(usuario, propuesta.Id, ct))
                throw new ConflictoException("Ya enviaste un presupuesto para esta propuesta.");

            var presupuesto = new ProfesionalPropuesta(usuario, propuesta.Id, request.Monto, request.Descripcion.Trim());
            presupuestos.Agregar(presupuesto);
            return presupuesto;
        }, ct);
        return PresupuestoResponse.DesdeEntidad(creado);
    }

    public async Task<IReadOnlyList<PresupuestoResponse>> RecibidosAsync(int usuario, int propuesta, CancellationToken ct)
    {
        await CuentaActivaAsync(usuario, ct);
        ExigirPropietario(await BuscarPropuestaAsync(propuesta, ct), usuario);
        var encontrados = await presupuestos.ObtenerDePropuestaAsync(propuesta, ct);
        return encontrados.Select(PresupuestoResponse.DesdeEntidad).ToList();
    }

    public async Task<IReadOnlyList<PresupuestoResponse>> MiosAsync(int usuario, CancellationToken ct)
    {
        await ProfesionalActivoAsync(usuario, ct);
        var encontrados = await presupuestos.ObtenerDeProfesionalAsync(usuario, ct);
        return encontrados.Select(PresupuestoResponse.DesdeEntidad).ToList();
    }

    public Task<PresupuestoResponse> AceptarAsync(int usuario, int presupuesto, CancellationToken ct) =>
        unidadDeTrabajo.EjecutarAsync(async () =>
        {
            await CuentaActivaAsync(usuario, ct);
            var oferta = await BuscarPresupuestoAsync(presupuesto, ct);
            var propuesta = await BuscarPropuestaAsync(oferta.IdPropuesta, ct);
            ExigirPropietario(propuesta, usuario);
            ExigirActiva(propuesta);
            ExigirPendiente(oferta);
            var profesional = await usuarios.ObtenerPorIdAsync(oferta.IdProfesional, ct);
            if (profesional is not Profesional { Activo: true } disponible)
                throw new ConflictoException("El profesional de este presupuesto ya no está activo.");
            await ExigirDisponibilidadAsync(disponible, ct);

            var pendientes = await presupuestos.ObtenerPendientesAsync(propuesta.Id, ct);
            foreach (var pendiente in pendientes)
                pendiente.RegistrarSeleccionDelUsuario(pendiente.Id == oferta.Id);
            return PresupuestoResponse.DesdeEntidad(oferta);
        }, ct);

    public Task<ConfirmacionPresupuestoResponse> ConfirmarAsync(int usuario, int presupuesto, CancellationToken ct) =>
        unidadDeTrabajo.EjecutarAsync(async () =>
        {
            var profesional = await ProfesionalActivoAsync(usuario, ct);
            var oferta = await BuscarPresupuestoAsync(presupuesto, ct);
            ExigirAutor(oferta, usuario);

            if (oferta.Estado == EstadoPresupuestoEnum.Aceptado)
            {
                var existente = await presupuestos.ObtenerIdEncargoAsync(oferta.Id, ct)
                    ?? throw new ConflictoException("El acuerdo no tiene un encargo asociado.");
                return new ConfirmacionPresupuestoResponse(PresupuestoResponse.DesdeEntidad(oferta), existente);
            }

            ExigirPendiente(oferta);
            if (!oferta.AprobadoUsuario)
                throw new ConflictoException("El usuario todavía no aceptó este presupuesto.");
            var propuesta = await BuscarPropuestaAsync(oferta.IdPropuesta, ct);
            ExigirActiva(propuesta);
            await ExigirDisponibilidadAsync(profesional, ct);
            if (await presupuestos.ExisteEncargoDePropuestaAsync(propuesta.Id, ct))
                throw new ConflictoException("La propuesta ya tiene un encargo.");

            var pendientes = await presupuestos.ObtenerPendientesAsync(propuesta.Id, ct);
            foreach (var otra in pendientes.Where(p => p.Id != oferta.Id))
                otra.Cancelar();
            oferta.ConfirmarAcuerdo();
            propuesta.Adjudicar();
            profesional.OcuparPorEncargo();
            var encargo = new Encargo(propuesta.Id, oferta.Id, propuesta.Ubicacion);
            var idEncargo = await presupuestos.CrearEncargoConChatAsync(encargo, propuesta.IdUsuario, usuario, ct);
            return new ConfirmacionPresupuestoResponse(PresupuestoResponse.DesdeEntidad(oferta), idEncargo);
        }, ct);

    public Task<PresupuestoResponse> RetirarAsync(int usuario, int presupuesto, CancellationToken ct) =>
        unidadDeTrabajo.EjecutarAsync(async () =>
        {
            await ProfesionalActivoAsync(usuario, ct);
            var oferta = await BuscarPresupuestoAsync(presupuesto, ct);
            ExigirAutor(oferta, usuario);
            if (oferta.Estado == EstadoPresupuestoEnum.Aceptado)
                throw new ConflictoException("El presupuesto ya generó un encargo.");
            oferta.Cancelar();
            return PresupuestoResponse.DesdeEntidad(oferta);
        }, ct);

    private async Task<Usuario> CuentaActivaAsync(int id, CancellationToken ct)
    {
        var usuario = await usuarios.ObtenerPorIdAsync(id, ct);
        if (usuario is null || !usuario.Activo)
            throw new UnauthorizedAccessException("La cuenta debe estar activa.");
        return usuario;
    }

    private async Task<Profesional> ProfesionalActivoAsync(int id, CancellationToken ct) =>
        await CuentaActivaAsync(id, ct) as Profesional
            ?? throw new UnauthorizedAccessException("Esta acción requiere un profesional.");

    private async Task ExigirDisponibilidadAsync(Profesional profesional, CancellationToken ct)
    {
        if (profesional.Estado != EstadoProfesionalEnum.Disponible ||
            await presupuestos.TieneEncargoActivoAsync(profesional.Id, ct))
            throw new ConflictoException("El profesional debe estar disponible y sin otro encargo activo.");
    }

    private async Task<Propuesta> BuscarPropuestaAsync(int id, CancellationToken ct) =>
        await propuestas.ObtenerPorIdAsync(id, ct) ?? throw new KeyNotFoundException("La propuesta no existe.");

    private async Task<ProfesionalPropuesta> BuscarPresupuestoAsync(int id, CancellationToken ct) =>
        await presupuestos.ObtenerAsync(id, ct) ?? throw new KeyNotFoundException("El presupuesto no existe.");

    private static void ExigirPropietario(Propuesta propuesta, int usuario)
    {
        if (propuesta.IdUsuario != usuario)
            throw new UnauthorizedAccessException("Solo el propietario puede gestionar los presupuestos recibidos.");
    }

    private static void ExigirAutor(ProfesionalPropuesta oferta, int usuario)
    {
        if (oferta.IdProfesional != usuario)
            throw new UnauthorizedAccessException("Solo el profesional que envió el presupuesto puede realizar esta acción.");
    }

    private static void ExigirActiva(Propuesta propuesta)
    {
        if (propuesta.Estado != EstadoPropuestaEnum.Activa)
            throw new ConflictoException("La propuesta debe estar activa.");
    }

    private static void ExigirPendiente(ProfesionalPropuesta oferta)
    {
        if (oferta.Estado != EstadoPresupuestoEnum.Pendiente)
            throw new ConflictoException("El presupuesto debe estar pendiente.");
    }
}
