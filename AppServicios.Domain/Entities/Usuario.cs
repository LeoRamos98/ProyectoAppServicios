using AppServicios.Domain.Enums;
namespace AppServicios.Domain.Entities
{
    public class Usuario
    {       
        public int Id { get; }
        public string Nombre { get; }
        public string Apellido { get; }
        public string Direccion { get; private set; }
        public string Email { get; }
        public string Contrasenia { get; }
        public string Telefono { get; }
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
