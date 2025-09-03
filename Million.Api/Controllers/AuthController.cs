using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Interfaces;
using Million.Domain.Entities;

namespace Million.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthentication _authenticationService;

        public AuthController(IAuthentication authentication)
        {
            _authenticationService = authentication;
        }

        /// <summary>
        /// Endpoint para autenticar al usuario y generar un token JWT.
        /// </summary>
        /// <param name="request">Objeto que contiene las credenciales del usuario.</param>
        /// <returns>
        /// Si las credenciales son válidas, devuelve un token JWT. 
        /// Si son incorrectas, retorna un código 401 (Unauthorized) con un mensaje de error.
        /// </returns>
        /// <response code="200">Token JWT generado correctamente.</response>
        /// <response code="401">Credenciales incorrectas.</response>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Intentamos autenticar al usuario
            var token = _authenticationService.Authenticate(request.Username, request.Password);

            if (token == null)
                return Unauthorized("Credenciales incorrectas.");

            // Si es válido, devolvemos el token JWT
            return Ok(new { Token = token });
        }
    }
}
