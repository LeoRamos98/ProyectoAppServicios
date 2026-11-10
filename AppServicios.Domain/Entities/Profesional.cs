using AppServicios.Domain.Enums;

namespace AppServicios.Domain.Entities
{
    public class Profesional : Usuario
    {
        public string Zona { get; private set; }
        public string TipoProfesional { get; private set; }
        public string NroMatricula { get; private set; }
        public string FotoPerfil { get; private set; } = string.Empty;

        public string ComentarioPresentacion { get; private set; } = string.Empty;

        public EstadoProfesionalEnum Estado { get; private set; }
            = EstadoProfesionalEnum.Disponible;

        public Profesional(
            string nombre,
            string apellido,
            string email,
            string contrasenia,
            string telefono,
            string direccion,
            string zona,
            string tipoProfesional,
            string nroMatricula)
            : base(nombre, apellido, email, contrasenia, telefono, direccion)
        {
            Rol = RolEnum.Profesional;
            Zona = zona;
            TipoProfesional = tipoProfesional;
            NroMatricula = nroMatricula;

        }
        public void ActualizarPresentacion(
        string fotoPerfil,
        string comentarioPresentacion)
        {
            FotoPerfil = fotoPerfil;
            ComentarioPresentacion = comentarioPresentacion;
        }
    }
}