using Mailer.DTOs;
using Mailer.Interface;
using Microsoft.AspNetCore.Mvc;
using TestMailer.Service;

namespace TestMailer.Controllers
{
    [Route("mailer")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly TestMailerService _mailerService;

        public MailController(TestMailerService mailerService)
        {
            _mailerService = mailerService;
        }

        public MailController(IMailerService @object)
        {
            Object = @object;
        }

        public IMailerService Object { get; }

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
                return BadRequest(ModelState);
            }
            try
            {
                var message =
                    $"Nom: {request.LastName}\nPrénom: {request.FirstName}\nEmail: {request.Email}\nDescription: {request.Description}";
                await _mailerService.SendMail(request.Subject, message, email, request.LastName);
                return Ok(new { Message = "Mail envoyé avec succés" });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "Erreur dans l'envoi du mail",
                        email,
                        Error = ex.Message
                    }
                );
            }
        }
    }
}
