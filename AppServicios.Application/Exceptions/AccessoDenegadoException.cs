namespace AppServicios.Application.Exceptions
{
    /// <summary>
    /// La persona que realiza la operaion no tiene permiso para hacerla.
    /// </summary>
    public class AccesoDenegadoException : Exception
    {
        public AccesoDenegadoException(string message) : base(message)
        {

        }
    }
}