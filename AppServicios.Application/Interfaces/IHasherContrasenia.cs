namespace AppServicios.Application.Interfaces
{
    public interface IHasherContrasenia
    {
        string Hashear(string contrasenia);

        bool Verificar(string contrasenia, string hash);
    }
}
