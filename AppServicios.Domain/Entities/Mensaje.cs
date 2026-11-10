namespace AppServicios.Domain.Entities
{
    public class Mensaje
    {
        public int Id { get; private set; }

        public int IdChat { get; private set; }

        public int IdUsuario { get; private set; }

        public string Texto { get; private set; }

        public DateTime FechaEnvio { get; private set; }

        public Mensaje(
            int idChat,
            int idUsuario,
            string texto)
        {
            IdChat = idChat;
            IdUsuario = idUsuario;
            Texto = texto;
            FechaEnvio = DateTime.UtcNow;
        }
    }
}