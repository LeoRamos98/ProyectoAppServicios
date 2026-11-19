using AppServicios.Domain.Entities;

namespace AppServicios.Application.Interfaces
{
    public interface IGeneradorToken
    {
        /// <summary>
        /// Genera el token de acceso del usuario con su id y su rol.
        /// </summary>
        TokenGenerado Generar(Usuario usuario);
    }

    public record TokenGenerado(string Token, DateTime ExpiraEn);
}