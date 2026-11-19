using AppServicios.Domain.Enums;

namespace AppServicios.Domain.Entities
{
    public class Encargo
    {
        public int Id { get; private set; }
        public int IdPropuesta { get; private set; }
        public int IdProfesionalPropuesta { get; private set; }

        public string Ubicacion { get; private set; }

        public EstadoEncargoEnum Estado { get; private set; }
            = EstadoEncargoEnum.PendienteDeInicio;

        public DateTime? FechaInicioEncargo { get; private set; }
        public DateTime? FechaFinalizacionEncargo { get; private set; }

        public Encargo(
            int idPropuesta,
            int idProfesionalPropuesta,
            string ubicacion)
        {
            IdPropuesta = idPropuesta;
            IdProfesionalPropuesta = idProfesionalPropuesta;
            Ubicacion = ubicacion;
        }

        /// <exception cref="InvalidOperationException">
        /// Si el encargo no esta pendiente de inicio..
        /// </exception>
        public void Iniciar()
        {
            if (Estado != EstadoEncargoEnum.PendienteDeInicio)
            {
                throw new InvalidOperationException(
                    "Solo se puede iniciar un encargo pendiente de inicio.");
            }

            Estado = EstadoEncargoEnum.EnCurso;
            FechaInicioEncargo = DateTime.UtcNow;
        }

        /// <exception>
        /// Finaliza la operación.
        /// </exception>
        public void Finalizar()
        {
            if (Estado != EstadoEncargoEnum.EnCurso)
            {
                throw new InvalidOperationException(
                    "Solo se puede finalizar un encargo en curso.");
            }

            Estado = EstadoEncargoEnum.Finalizado;
            FechaFinalizacionEncargo = DateTime.UtcNow;
        }

        /// <exception>
        /// Si el encargo ya esta finalizado o cancelado.
        /// </exception>
        public void Cancelar()
        {
            if (Estado == EstadoEncargoEnum.Finalizado || Estado == EstadoEncargoEnum.Cancelado)
            {
                throw new InvalidOperationException(
                    "No se puede cancelar el encargo finalizado o ya cancelado.");
            }

            Estado = EstadoEncargoEnum.Cancelado;
        }
    }
}