using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppServicios.Infrastructure.Security
{
    /// <summary>
    /// Configuración de los tokens JWT (sección "Jwt" de la configuración).
    /// La clave NO va en appsettings.json: se guarda en user-secrets en
    /// desarrollo o en una variable de entorno (Jwt__Clave) en el servidor.
    /// </summary>
    public class JwtOpciones
    {
        public const string Seccion = "Jwt";

        /// <summary>HS256 exige una clave de al menos 256 bits.</summary>
        public const int LongitudMinimaClaveBytes = 32;

        [Required]
        public string Emisor { get; set; } = string.Empty;

        [Required]
        public string Audiencia { get; set; } = string.Empty;

        [Required]
        public string Clave { get; set; } = string.Empty;

        [Range(1, 1440)]
        public int MinutosExpiracion { get; set; } = 60;

        public byte[] ClaveEnBytes() => Encoding.UTF8.GetBytes(Clave);

        public bool ClaveEsSuficiente() =>
            ClaveEnBytes().Length >= LongitudMinimaClaveBytes;
    }
}