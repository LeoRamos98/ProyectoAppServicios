using AppServicios.Domain.Enums;

namespace AppServicios.Domain.Entities
{
    public class Propuesta
    {
        public int Id { get; private set; }
        public int IdUsuario { get; private set; }

        public string TipoServicio { get; private set; }
        public string Descripcion { get; private set; }
        public string Ubicacion { get; private set; }
        public double? Latitud { get; private set; }
        public double? Longitud { get; private set; }
        public string FotoPropuesta { get; private set; }

        public DateTime FechaSolicitada { get; private set; }

        public EstadoPropuestaEnum Estado { get; private set; }
            = EstadoPropuestaEnum.Activa;

        public Propuesta(
            int idUsuario,
            string tipoServicio,
            string descripcion,
            string ubicacion,
            DateTime fechaSolicitada,
            string? fotoPropuesta = null)
        {
            IdUsuario = idUsuario;
            TipoServicio = tipoServicio;
            Descripcion = descripcion;
            Ubicacion = ubicacion;
            FechaSolicitada = fechaSolicitada;
            FotoPropuesta = fotoPropuesta ?? string.Empty;
        }

        public void Actualizar(
            string tipoServicio, string descripcion, string ubicacion,
            double latitud, double longitud, DateTime fechaSolicitada, string? foto)
        {
            ConfirmarUbicacion(ubicacion, latitud, longitud);
            TipoServicio = tipoServicio.Trim();
            Descripcion = descripcion.Trim();
            FechaSolicitada = fechaSolicitada;
            FotoPropuesta = foto?.Trim() ?? string.Empty;
        }

        public void Adjudicar() => Estado = EstadoPropuestaEnum.Adjudicada;

        public void Cancelar() => Estado = EstadoPropuestaEnum.Cancelada;

        public void ConfirmarUbicacion(string ubicacion, double latitud, double longitud)
        {
            if (string.IsNullOrWhiteSpace(ubicacion))
                throw new ArgumentException("La dirección es obligatoria.", nameof(ubicacion));

            if (!double.IsFinite(latitud) || latitud < -90 || latitud > 90)
                throw new ArgumentOutOfRangeException(nameof(latitud), "La latitud debe estar entre -90 y 90.");

            if (!double.IsFinite(longitud) || longitud < -180 || longitud > 180)
                throw new ArgumentOutOfRangeException(nameof(longitud), "La longitud debe estar entre -180 y 180.");

            Ubicacion = ubicacion.Trim();
            Latitud = latitud;
            Longitud = longitud;
        }
    }
}