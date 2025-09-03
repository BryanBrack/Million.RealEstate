using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Million.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Million.Infrastructure.Services
{
    public class AuthenticationService : IAuthentication
    {
        private readonly IConfiguration _configuration;

        public AuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Usuario y contraseña predefinidos
        private const string PredefinedUsername = "admin";
        private const string PredefinedPassword = "password123";

        // Llave secreta para firmar el JWT
        private const string SecretKey = "tu_clave_secreta_de_32_bytes_asegurada_que_es_256_bits";

        /// <summary>
        /// Método para autenticar al usuario y generar un token JWT.
        /// </summary>
        /// <param name="username">Nombre de usuario proporcionado por el usuario.</param>
        /// <param name="password">Contraseña proporcionada por el usuario.</param>
        /// <returns>Token JWT si las credenciales son correctas, o null si no lo son.</returns>
        public string Authenticate(string username, string password)
        {
            // Verificamos las credenciales
            if (username == PredefinedUsername && password == PredefinedPassword)
            {
                return GenerateJwtToken(username);
            }
            else
            {
                Console.WriteLine("Usuario o contraseña incorrectos.");
                return null;
            }
        }

        /// <summary>
        /// Método para generar un token JWT.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <returns>Token JWT generado.</returns>
        private string GenerateJwtToken(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
            };

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var signingKey = new SymmetricSecurityKey(keyBytes);

            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
