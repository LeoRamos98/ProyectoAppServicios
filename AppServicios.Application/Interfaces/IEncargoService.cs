using AppServicios.Application.DTOs.Encargos;

namespace AppServicios.Application.Interfaces
{
    public interface IEncargoService
    {
        Task<IReadOnlyList<EncargoResponse>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task<EncargoResponse?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EncargoResponse>> ObtenerPorUsuarioAsync(
            int idUsuario,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EncargoResponse>> ObtenerPorProfesionalAsync(
            int idProfesional,
            CancellationToken cancellationToken = default);

        /// <exception cref="Exceptions.ConflictoException">
        /// Si el presupuesto no está aprobado por ambas partes, está cancelado,
        /// la propuesta no está activa o ya tiene un encargo.
        /// </exception>
        Task<EncargoResponse?> CrearAsync(
            CrearEncargoRequest request,
            CancellationToken cancellationToken = default);

        /// <exception cref="Exceptions.AccesoDenegadoException">
        /// Si quien inicia no es el profesional asignado.
        /// </exception>
        /// <exception cref="Exceptions.ConflictoException">
        /// Si el encargo no está pendiente de inicio.
        /// </exception>
        Task<EncargoResponse?> IniciarAsync(
            int id,
            AccionEncargoRequest request,
            CancellationToken cancellationToken = default);

        /// <exception cref="Exceptions.AccesoDenegadoException">
        /// Si quien finaliza no es el profesional asignado.
        /// </exception>
        /// <exception cref="Exceptions.ConflictoException">
        /// Si el encargo no está en curso.
        /// </exception>
        Task<EncargoResponse?> FinalizarAsync(
            int id,
            AccionEncargoRequest request,
            CancellationToken cancellationToken = default);

        /// <exception cref="Exceptions.AccesoDenegadoException">
        /// Si quien cancela no es el usuario solicitante ni el profesional asignado.
        /// </exception>
        /// <exception cref="Exceptions.ConflictoException">
        /// Si el encargo ya está finalizado o cancelado.
        /// </exception>
        Task<EncargoResponse?> CancelarAsync(
            int id,
            AccionEncargoRequest request,
            CancellationToken cancellationToken = default);
    }
}
}