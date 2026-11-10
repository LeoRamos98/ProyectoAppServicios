using AppServicios.Domain.Enums;

namespace AppServicios.Domain.Entities
{
    public class ProfesionalPropuesta
    {
        public int Id { get; private set; }
        public int IdProfesional { get; private set; }
        public int IdPropuesta { get; private set; }

        public decimal Monto { get; private set; }
        public string Descripcion { get; private set; }
        public DateTime FechaPresupuesto { get; private set; }

        public bool AprobadoUsuario { get; private set; } = false;
        public bool AprobadoProfesional { get; private set; } = false;

        public EstadoPresupuestoEnum Estado { get; private set; }
            = EstadoPresupuestoEnum.Pendiente;

        public ProfesionalPropuesta(
            int idProfesional,
            int idPropuesta,
            decimal monto,
            string descripcion)
        {
            IdProfesional = idProfesional;
            IdPropuesta = idPropuesta;
            Monto = monto;
            Descripcion = descripcion;
            FechaPresupuesto = DateTime.UtcNow;
        }
    }
}