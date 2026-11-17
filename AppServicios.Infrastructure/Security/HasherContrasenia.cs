using System.Security.Cryptography;
using AppServicios.Application.Interfaces;

namespace AppServicios.Infrastructure.Security
{
    /// <summary>
    /// Hash de contraseñas con PBKDF2 (SHA-256) y sal aleatoria.
    /// Formato almacenado: "{iteraciones}.{salBase64}.{hashBase64}".
    /// </summary>
    public class HasherContrasenia : IHasherContrasenia
    {
        private const int TamanioSal = 16;
        private const int TamanioHash = 32;
        private const int Iteraciones = 100_000;
        private static readonly HashAlgorithmName Algoritmo = HashAlgorithmName.SHA256;

        public string Hashear(string contrasenia)
        {
            var sal = RandomNumberGenerator.GetBytes(TamanioSal);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                contrasenia, sal, Iteraciones, Algoritmo, TamanioHash);

            return $"{Iteraciones}.{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
        }

        public bool Verificar(string contrasenia, string hash)
        {
            var partes = hash.Split('.');
            if (partes.Length != 3 || !int.TryParse(partes[0], out var iteraciones))
            {
                return false;
            }

            var sal = Convert.FromBase64String(partes[1]);
            var esperado = Convert.FromBase64String(partes[2]);
            var calculado = Rfc2898DeriveBytes.Pbkdf2(
                contrasenia, sal, iteraciones, Algoritmo, esperado.Length);

            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }
    }
}
