using AppServicios.Application.DTOs.Encargos;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;
using AppServicios.Domain.Interfaces;
using AppServicios.Domain.Models;

namespace AppServicios.Application.Services
{
    public class EncargoService : IEncargoService
    {
        private readonly IEncargoRepository _encargoRepository;
        private readonly IProfesionalPropuestaRepository _presupuestos;
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;

        public EncargoService(
            IEncargoRepository encargoRepository,
            IProfesionalPropuestaRepository presupuestos,
            IUsuarioRepository usuarios,
            IUnidadDeTrabajo unidadDeTrabajo)
        {
            _encargoRepository = encargoRepository;
            _presupuestos = presupuestos;
            _usuarios = usuarios;
            _unidadDeTrabajo = unidadDeTrabajo;
        }

        public async Task<IReadOnlyList<EncargoResponse>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            var encargos = await _encargoRepository
                .ObtenerTodosAsync(cancellationToken);

            return encargos.Select(EncargoResponse.DesdeDetalle).ToList();
        }

        public async Task<EncargoResponse?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var encargo = await _encargoRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            return encargo is null ? null : EncargoResponse.DesdeDetalle(encargo);
        }

        public async Task<IReadOnlyList<EncargoResponse>> ObtenerPorUsuarioAsync(
            int idUsuario,
            CancellationToken cancellationToken = default)
        {
            var encargos = await _encargoRepository
                .ObtenerPorUsuarioAsync(idUsuario, cancellationToken);

            return encargos.Select(EncargoResponse.DesdeDetalle).ToList();
        }

        public async Task<IReadOnlyList<EncargoResponse>> ObtenerPorProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default)
        {
            var encargos = await _encargoRepository
                .ObtenerPorProfesionalAsync(idProfesional, cancellationToken);

            return encargos.Select(EncargoResponse.DesdeDetalle).ToList();
        }

        public async Task<EncargoResponse?> CrearAsync(
            CrearEncargoRequest request,
            CancellationToken cancellationToken = default)
        {
            var presupuesto = await _encargoRepository.ObtenerPresupuestoAsync(
                request.IdProfesionalPropuesta, cancellationToken);

            if (presupuesto is null)
            {
                return null;
            }

            if (presupuesto.Estado == EstadoPresupuestoEnum.Cancelado)
            {
                throw new ConflictoException(
                    "El presupuesto está cancelado y no puede originar un encargo.");
            }

            if (!presupuesto.AprobadoUsuario || !presupuesto.AprobadoProfesional)
            {
                throw new ConflictoException(
                    "El presupuesto debe estar aprobado por el usuario y por el " +
                    "profesional para generar el encargo.");
            }

            var propuesta = await _encargoRepository.ObtenerPropuestaAsync(
                presupuesto.IdPropuesta, cancellationToken);

            if (propuesta is null)
            {
                throw new ConflictoException(
                    "La propuesta asociada al presupuesto no existe.");
            }

            if (propuesta.Estado != EstadoPropuestaEnum.Activa)
            {
                throw new ConflictoException(
                    "La propuesta no está activa y no puede originar un encargo.");
            }

            if (await _encargoRepository.ExisteParaPropuestaAsync(
                    propuesta.Id, cancellationToken))
            {
                throw new ConflictoException(
                    "La propuesta ya tiene un encargo generado.");
            }

            var encargo = new Encargo(
                propuesta.Id,
                presupuesto.Id,
                propuesta.Ubicacion);

            await _encargoRepository.AgregarAsync(encargo, cancellationToken);
            await _encargoRepository.GuardarCambiosAsync(cancellationToken);

            return EncargoResponse.DesdeDetalle(new EncargoDetalle(
                encargo,
                propuesta.IdUsuario,
                presupuesto.IdProfesional));
        }

        public Task<EncargoResponse?> IniciarAsync(
            int id,
            AccionEncargoRequest request,
            CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                id,
                request.IdUsuario,
                soloProfesionalAsignado: true,
                mensajeSinPermiso:
                    "Solo el profesional asignado puede iniciar el encargo.",
                transicion: encargo => encargo.Iniciar(),
                cancellationToken);
        }

        public Task<EncargoResponse?> FinalizarAsync(
            int id,
            AccionEncargoRequest request,
            CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                id,
                request.IdUsuario,
                soloProfesionalAsignado: true,
                mensajeSinPermiso:
                    "Solo el profesional asignado puede finalizar el encargo.",
                transicion: encargo => encargo.Finalizar(),
                cancellationToken);
        }

        public Task<EncargoResponse?> CancelarAsync(
            int id,
            AccionEncargoRequest request,
            CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                id,
                request.IdUsuario,
                soloProfesionalAsignado: false,
                mensajeSinPermiso:
                    "Solo el usuario solicitante o el profesional asignado " +
                    "pueden cancelar el encargo.",
                transicion: encargo => encargo.Cancelar(),
                cancellationToken);
        }

        /// <summary>
        /// Flujo común de iniciar, finalizar y cancelar: busca el encargo,
        /// valida quién lo pide, aplica la transición y guarda.
        /// </summary>
        private Task<EncargoResponse?> CambiarEstadoAsync(
            int id,
            int idUsuario,
            bool soloProfesionalAsignado,
            string mensajeSinPermiso,
            Action<Encargo> transicion,
            CancellationToken cancellationToken)
        {
            return _unidadDeTrabajo.EjecutarAsync<EncargoResponse?>(async () =>
            {
                var detalle = await _encargoRepository
                    .ObtenerPorIdAsync(id, cancellationToken);

                if (detalle is null)
                {
                    return null;
                }

                var tienePermiso = soloProfesionalAsignado
                    ? idUsuario == detalle.IdProfesional
                    : idUsuario == detalle.IdProfesional
                        || idUsuario == detalle.IdUsuario;

                if (!tienePermiso)
                {
                    throw new AccesoDenegadoException(mensajeSinPermiso);
                }

                try
                {
                    transicion(detalle.Encargo);
                }
                catch (InvalidOperationException ex)
                {
                    throw new ConflictoException(ex.Message);
                }

                if (detalle.Encargo.Estado is EstadoEncargoEnum.Finalizado or EstadoEncargoEnum.Cancelado)
                {
                    // La consulta de encargos activos debe ver este cierre dentro de la misma transacción.
                    await _encargoRepository.GuardarCambiosAsync(cancellationToken);
                    await LiberarProfesionalSiCorrespondeAsync(detalle.IdProfesional, cancellationToken);
                }

                return EncargoResponse.DesdeDetalle(detalle);
            }, cancellationToken);
        }

        private async Task LiberarProfesionalSiCorrespondeAsync(
            int idProfesional, CancellationToken cancellationToken)
        {
            if (await _presupuestos.TieneEncargoActivoAsync(idProfesional, cancellationToken))
            {
                return;
            }

            var usuario = await _usuarios.ObtenerPorIdAsync(idProfesional, cancellationToken);
            if (usuario is not Profesional profesional)
            {
                throw new ConflictoException("No se encontró el profesional asignado al encargo.");
            }

            profesional.LiberarPorEncargo();
        }
    }
}