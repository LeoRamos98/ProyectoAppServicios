namespace AppServicios.Domain.Entities
{
    public class Chat
    {
        public int Id { get; private set; }

        public int IdEncargo { get; private set; }

        public int IdUsuario { get; private set; }

        public int IdProfesional { get; private set; }

        public DateTime FechaCreacion { get; private set; }

        public Chat(
            int idEncargo,
            int idUsuario,
            int idProfesional)
        {
            IdEncargo = idEncargo;
            IdUsuario = idUsuario;
            IdProfesional = idProfesional;
            FechaCreacion = DateTime.UtcNow;
        }
    }
}