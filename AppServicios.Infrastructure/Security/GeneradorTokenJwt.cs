using System.Security.Claims;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AppServicios.Infrastructure.Security
{
    /// <summary>
    /// Genera tokens JWT firmados con HMAC-SHA256.
    /// El id del usuario viaja en ClaimTypes.NameIdentifier (lo lee
    /// ControladorAutenticado) y el rol en ClaimTypes.Role
    /// (lo usa [Authorize(Roles = ...)]).
    /// </summary>
    public class GeneradorTokenJwt : IGeneradorToken
    {
        private readonly JwtOpciones _opciones;
        private readonly SigningCredentials _credenciales;
        private readonly JsonWebTokenHandler _manejador = new();

        public GeneradorTokenJwt(IOptions<JwtOpciones> opciones)
        {
            _opciones = opciones.Value;
            _credenciales = new SigningCredentials(
                new SymmetricSecurityKey(_opciones.ClaveEnBytes()),
                SecurityAlgorithms.HmacSha256);
        }

        public TokenGenerado Generar(Usuario usuario)
        {
            var ahora = DateTime.UtcNow;
            var expiraEn = ahora.AddMinutes(_opciones.MinutosExpiracion);
            var id = usuario.Id.ToString();

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _opciones.Emisor,
                Audience = _opciones.Audiencia,
                IssuedAt = ahora,
                NotBefore = ahora,
                Expires = expiraEn,
                SigningCredentials = _credenciales,
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString())
                ])
            };

            return new TokenGenerado(_manejador.CreateToken(descriptor), expiraEn);
        }
    }
}