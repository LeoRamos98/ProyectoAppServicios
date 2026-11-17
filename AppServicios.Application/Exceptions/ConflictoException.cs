namespace AppServicios.Application.Exceptions
{
    /// <summary>
    /// La operación choca con el estado actual de los datos
    /// (por ejemplo, un email ya registrado).
    /// </summary>
    public class ConflictoException : Exception
    {
        public ConflictoException(string message) : base(message)
        {
        }
    }
}
