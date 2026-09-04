namespace ProyectoAppServicios.AppServicios.Domain.Entities
{
    public class Usuario
    {
        private static int siguienteId = 1;
        public Guid Id { get; }
        public string Nombre { get; }
        public string Apellido { get; }
        public string Email { get; }
        public string Contrasenia { get; }
        public string Telefono { get; }

        public Usuario(string nombre, string apellido, string email, string contrasenia, string telefono)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Apellido = apellido;
            Email = email;
            Contrasenia = contrasenia;
            Telefono = telefono;
        }
    }
}
