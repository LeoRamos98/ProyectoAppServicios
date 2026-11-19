using AppServicios.Application.DTOs.Autenticacion;

namespace AppServicios.Application.Interfaces
{
    public interface IAutenticacionService
    {
        /// <returns>
        /// El token y los datos del usuario, o null si el email no existe,
        /// la contraseña es incorrecta o la cuenta está inactiva.
        /// </returns>
        Task<LoginResponse?> IniciarSesionAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default);
    }
}