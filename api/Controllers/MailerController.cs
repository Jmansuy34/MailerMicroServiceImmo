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
        private readonly EmailTemplateService _emailTemplateService;

        public MailerController(
            MailerService mailerService,
            EmailTemplateService emailTemplateService
        )
        {
            _mailerService = mailerService;
            _emailTemplateService = emailTemplateService; // Initialisation du service de template
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromBody] EmailRequest request)
        {
            // Extraire l'email de la requête au lieu du contexte
            var email = request.Email;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (string.IsNullOrEmpty(email))
            {
                return BadRequest(
                    new { Message = "Adresse email non trouvée dans le corps de la requête." }
                );
            }

            try
            {
                // Charger le template d'email
                var emailBody = await _emailTemplateService.GetSimpleEmailTemplate(
                    request.LastName,
                    request.FirstName,
                    request.Email,
                    request.Description
                );

                // Envoyer l'email en utilisant le template
                await _mailerService.SendMail(request.Subject, emailBody, email, request.LastName);
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
