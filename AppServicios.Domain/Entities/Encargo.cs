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
    }
}