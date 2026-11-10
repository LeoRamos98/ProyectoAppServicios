using AppServicios.Domain.Enums;
namespace AppServicios.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; private set; }

        public string Nombre { get; private set; }

        public string Apellido { get; private set; }

        public string Direccion { get; private set; }

        public string Email { get; private set; }

        public string Contrasenia { get; private set; }

        public string Telefono { get; private set; }

        public RolEnum Rol { get; protected set; } = RolEnum.Usuario;

        public bool Activo { get; private set; } = true;

        public Usuario(string nombre, string apellido, string email, string contrasenia, string telefono, string direccion)
        {
            Nombre = nombre;
            Apellido = apellido;
            Email = email;
            Contrasenia = contrasenia;
            Telefono = telefono;
            Direccion = direccion;
        }
    }
}
