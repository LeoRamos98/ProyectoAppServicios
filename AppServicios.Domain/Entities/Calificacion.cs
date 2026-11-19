namespace AppServicios.Domain.Entities
{
    /// <summary>
    /// Calificación que el usuario solicitante le deja al profesional
    /// una vez finalizado el encargo. Hay una sola calificación por encargo.
    /// </summary>
    public class Calificacion
    {
        public const int PuntajeMinimo = 1;
        public const int PuntajeMaximo = 5;
        public const int LongitudMaximaComentario = 500;

        public int Id { get; private set; }

        public int IdEncargo { get; private set; }

        public int IdProfesional { get; private set; }

        public int IdUsuario { get; private set; }

        public int Puntaje { get; private set; }

        public string Comentario { get; private set; }

        public DateTime FechaCalificacion { get; private set; }

        /// <exception cref="ArgumentOutOfRangeException">
        /// Si el puntaje no está entre 1 y 5.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Si el comentario supera la longitud máxima.
        /// </exception>
        public Calificacion(
            int idEncargo,
            int idProfesional,
            int idUsuario,
            int puntaje,
            string? comentario)
        {
            if (puntaje < PuntajeMinimo || puntaje > PuntajeMaximo)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(puntaje),
                    $"El puntaje debe estar entre {PuntajeMinimo} y {PuntajeMaximo}.");
            }

            var comentarioNormalizado = comentario?.Trim() ?? string.Empty;

            if (comentarioNormalizado.Length > LongitudMaximaComentario)
            {
                throw new ArgumentException(
                    $"El comentario no puede superar los {LongitudMaximaComentario} caracteres.",
                    nameof(comentario));
            }

            IdEncargo = idEncargo;
            IdProfesional = idProfesional;
            IdUsuario = idUsuario;
            Puntaje = puntaje;
            Comentario = comentarioNormalizado;
            FechaCalificacion = DateTime.UtcNow;
        }
    }
}
