using Microsoft.AspNetCore.Mvc;
using Mailer.Models;
using Mailer.Service;

namespace Mailer.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _tokenService;
        private readonly TwoFactorAuthService _twoFactorAuthService;
        private readonly MailerService _mailerService;

        public AuthController(JwtTokenService tokenService, TwoFactorAuthService twoFactorAuthService, MailerService mailerService)
        {
            _tokenService = tokenService;
            _twoFactorAuthService = twoFactorAuthService;
            _mailerService = mailerService;
        }

        // Endpoint de login avec génération du code 2FA
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Authentification simple pour cet exemple
            if (request.Email == "jmansuy@smbindustries.fr" && request.Password == "zexm adof rncy ctdq")
            {
                // Générer le code 2FA
                string code2FA = _twoFactorAuthService.GenerateAndStore2FACode(request.Email);

                // Envoyer le code par email
                await _mailerService.Send2FAEmail(request.Email, "Jayson", code2FA);

                // Retourner une réponse indiquant que le code 2FA a été envoyé
                return Ok(new { Message = "Un code 2FA a été envoyé à votre email. Veuillez le valider." });
            }
            else
            {
                return Unauthorized("Email ou mot de passe invalide.");
            }
        }

        // Endpoint pour valider le code 2FA et générer le JWT
        [HttpPost("validate-2fa")]
        public IActionResult Validate2FA([FromBody] TwoFactorRequest request)
        {
            if (_twoFactorAuthService.Validate2FACode(request.Email, request.Code))
            {
                // Si le code est valide, générer un jeton JWT
                string token = _tokenService.GenerateToken(request.Email);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized("Code 2FA invalide ou expiré.");
            }
        }
    }
}
