using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.DTOs
{
    public static class JwtSecretKey
    {
        public static byte[] GenerateSecretKey()
        {
            // Crear una clave aleatoria de 256 bits (32 bytes)
            using (var hmacsha256 = new HMACSHA256())
            {
                // Generar una clave segura de 256 bits
                return hmacsha256.Key; // La propiedad 'Key' contiene 256 bits (32 bytes)
            }
        }
    }
}
