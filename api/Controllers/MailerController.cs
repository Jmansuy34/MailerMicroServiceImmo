using System.Threading.Tasks;
using Mailer.DTOs;
using Mailer.Service;
using Microsoft.AspNetCore.Mvc;

namespace Mailer.Controllers
{
    [Route("mailer")]
    [ApiController]
    public class MailerController : ControllerBase
    {
        private readonly MailerService _mailerService;

        public MailerController(MailerService mailerService)
        {
            _mailerService = mailerService;
        }

        // Endpoint pour envoyer un email simple
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromBody] EmailRequest request)
        {
            var email = HttpContext.Items["Email"] as string;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { Message = "Adresse email non trouvée dans le contexte." });
            }

            try
            {
                var message =
                    $"Nom: {request.LastName}\nPrénom: {request.FirstName}\nEmail: {request.Email}\nDescription: {request.Description}";
                await _mailerService.SendMail(request.Subject, message, email, request.LastName);
                return Ok(new { Message = "Mail envoyé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "Erreur dans l'envoi du mail",
                        email,
                        Error = ex.Message,
                    }
                );
            }
        }

        // Nouveau endpoint pour simuler une inscription et envoyer un email de confirmation
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] EmailRequest request)
        {
            // Simuler la création d'un utilisateur ou d'une inscription
            var urlLink = "https://yourwebsite.com/confirm?token=someGeneratedToken"; // Générer dynamiquement un lien de confirmation d'inscription

            try
            {
                // Utiliser le service pour envoyer un email d'inscription
                await _mailerService.SendRegistrationEmail(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    urlLink
                );
                return Ok(
                    new { Message = "Email de confirmation d'inscription envoyé avec succès" }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "Erreur dans l'envoi de l'email",
                        request.Email,
                        Error = ex.Message,
                    }
                );
            }
        }

        [HttpPost("passwordReset")]
        public async Task<IActionResult> SendPasswordResetEmail([FromBody] EmailRequest request)
        {
            var resetLink = "https://yourwebsite.com/reset-password?token=someGeneratedToken"; // Générer dynamiquement un lien de réinitialisation

            try
            {
                await _mailerService.SendPasswordResetEmail(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    resetLink
                );
                return Ok(
                    new { Message = "Email de réinitialisation de mot de passe envoyé avec succès" }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "Erreur dans l'envoi de l'email",
                        request.Email,
                        Error = ex.Message,
                    }
                );
            }
        }
    }
}
