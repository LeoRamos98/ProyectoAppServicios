namespace AppServicios.Domain.Entities
{
    public class Calificacion
    {
        public int Id { get; private set; }

        public int IdEncargo { get; private set; }

        public int IdProfesional { get; private set; }

        public int IdUsuario { get; private set; }

        public int Puntaje { get; private set; }

        public string Comentario { get; private set; }

        public DateTime FechaCalificacion { get; private set; }

        public Calificacion(
            int idEncargo,
            int idProfesional,
            int idUsuario,
            int puntaje,
            string comentario)
        {
            IdEncargo = idEncargo;
            IdProfesional = idProfesional;
            IdUsuario = idUsuario;
            Puntaje = puntaje;
            Comentario = comentario;
            FechaCalificacion = DateTime.UtcNow;
        }
    }
}
