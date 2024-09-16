using Microsoft.AspNetCore.Mvc;
using Mailer.Models;  // <-- Ajoutez cette ligne pour inclure LoginRequest

namespace Mailer.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _tokenService;

        public AuthController(JwtTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Authentification simple pour cet exemple
            if (request.Email == "jmansuy@smbindustries.fr" && request.Password == "zexm adof rncy ctdq")
            {
                // Générer un jeton JWT
                string token = _tokenService.GenerateToken(request.Email);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized("Email ou mot de passe invalide.");
            }
        }
    }
}
