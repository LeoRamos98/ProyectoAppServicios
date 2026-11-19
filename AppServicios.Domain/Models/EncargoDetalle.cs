using AppServicios.Domain.Entities;

namespace AppServicios.Domain.Models
{
    /// <summary>
    /// Encargo junto con las dos partes involucradas. Encargo no guarda directamente al usuario
    /// ni al profesional: el usuario sale dee la Propuesta y el profesional del presupuesto (ProfesionalPropuesta).
    /// </summary>
    public record EncargoDetalle(
        Encargo Encargo,
        int IdUsuario,
        int IdProfesional);
}
