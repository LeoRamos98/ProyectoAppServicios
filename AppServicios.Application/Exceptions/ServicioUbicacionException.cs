namespace AppServicios.Application.Exceptions
{
    public class ServicioUbicacionException : Exception
    {
        public ServicioUbicacionException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
