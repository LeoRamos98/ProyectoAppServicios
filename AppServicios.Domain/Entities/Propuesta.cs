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
        public string FotoPropuesta { get; private set; } = string.Empty;

        public DateTime FechaSolicitada { get; private set; }

        public EstadoPropuestaEnum Estado { get; private set; }
            = EstadoPropuestaEnum.Activa;

        public Propuesta(
            int idUsuario,
            string tipoServicio,
            string descripcion,
            string ubicacion,
            DateTime fechaSolicitada)
        {
            IdUsuario = idUsuario;
            TipoServicio = tipoServicio;
            Descripcion = descripcion;
            Ubicacion = ubicacion;
            FechaSolicitada = fechaSolicitada;
        }
    }
}