using AppServicios.Application.DTOs.Encargos;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Entities;
using AppServicios.Domain.Enums;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class EncargoService : IEncargoService
    {
        private readonly IEncargoRepository _encargoRepository;
        private readonly IProfesionalPropuestaRepository _presupuestos;
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IProfesionalPropuestaService _acuerdos;

        public EncargoService(
            IEncargoRepository encargoRepository,
            IProfesionalPropuestaRepository presupuestos,
            IUsuarioRepository usuarios,
            IUnidadDeTrabajo unidadDeTrabajo,
            IProfesionalPropuestaService acuerdos)
        {
            _encargoRepository = encargoRepository;
            _presupuestos = presupuestos;
            _usuarios = usuarios;
            _unidadDeTrabajo = unidadDeTrabajo;
            _acuerdos = acuerdos;
        }

        public async Task<IReadOnlyList<EncargoResponse>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            var encargos = await _encargoRepository
                .ObtenerTodosAsync(cancellationToken);

            return encargos.Select(EncargoResponse.DesdeDetalle).ToList();
        }

        public async Task<EncargoResponse?> ObtenerPorIdAsync(
            int id, int idActor,
            CancellationToken cancellationToken = default)
        {
            var encargo = await _encargoRepository
                .ObtenerPorIdAsync(id, cancellationToken);

            if (encargo is null)
                return null;
            if (idActor != encargo.IdUsuario && idActor != encargo.IdProfesional &&
                !await EsAdministradorAsync(idActor, cancellationToken))
                throw new AccesoDenegadoException("Solo los participantes o un administrador pueden consultar el encargo.");

            return EncargoResponse.DesdeDetalle(encargo);
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
            int idActor, CrearEncargoRequest request,
            CancellationToken cancellationToken = default)
        {
            // La ruta de encargos confirma la misma oferta y reutiliza la transacción de presupuestos.
            var acuerdo = await _acuerdos.ConfirmarAsync(idActor, request.IdProfesionalPropuesta, cancellationToken);
            if (!acuerdo.EncargoCreado)
                throw new ConflictoException("El presupuesto ya tiene un encargo generado.");
            return await ObtenerPorIdAsync(acuerdo.IdEncargo, idActor, cancellationToken);
        }

        public Task<EncargoResponse?> IniciarAsync(
            int id,
            int idActor,
            CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                id,
                idActor,
                soloProfesionalAsignado: true,
                mensajeSinPermiso:
                    "Solo el profesional asignado puede iniciar el encargo.",
                transicion: encargo => encargo.Iniciar(),
                cancellationToken);
        }

        public Task<EncargoResponse?> FinalizarAsync(
            int id,
            int idActor,
            CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                id,
                idActor,
                soloProfesionalAsignado: true,
                mensajeSinPermiso:
                    "Solo el profesional asignado puede finalizar el encargo.",
                transicion: encargo => encargo.Finalizar(),
                cancellationToken);
        }

        public Task<EncargoResponse?> CancelarAsync(
            int id,
            int idActor,
            CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                id,
                idActor,
                soloProfesionalAsignado: false,
                mensajeSinPermiso:
                    "Solo los participantes o un administrador pueden cancelar el encargo.",
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

                var tienePermiso = idUsuario == detalle.IdProfesional ||
                    (!soloProfesionalAsignado && (idUsuario == detalle.IdUsuario ||
                        await EsAdministradorAsync(idUsuario, cancellationToken)));

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

        private async Task<bool> EsAdministradorAsync(int id, CancellationToken cancellationToken) =>
            await _usuarios.ObtenerRolActivoAsync(id, cancellationToken) == RolEnum.Admin;

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