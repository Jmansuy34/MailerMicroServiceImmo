using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Mailer.Service
{
    public class MailerService
    {
        private readonly string _emailAddress;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;

        public MailerService(IConfiguration configuration)
        {
            _emailAddress = configuration["EmailSettings:EmailAddress"];
            _password = configuration["EmailSettings:Password"];
            _host = configuration["EmailSettings:Host"];
            _port = int.Parse(configuration["EmailSettings:Port"]);
            _enableSsl = bool.Parse(configuration["EmailSettings:EnableSSL"]);
        }

        public async Task SendMail(
            string subject,
            string htmlContent, // Modifié pour accepter du HTML
            string receiverMail,
            string lastName
        )
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("IMMO-IA", _emailAddress));
            emailMessage.To.Add(new MailboxAddress(lastName, receiverMail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlContent }; // Envoyer le contenu HTML

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                        _host,
                        _port,
                        MailKit.Security.SecureSocketOptions.StartTls
                    );
                    await client.AuthenticateAsync(_emailAddress, _password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task SendRegistrationEmail(
            string receiverEmail,
            string firstName,
            string lastName,
            string urlLink
        )
        {
            var templateService = new EmailTemplateService();
            var emailBody = await templateService.GetRegistrationEmailTemplate(
                firstName,
                lastName,
                urlLink,
                "VotreEntreprise"
            );

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("VotreEntreprise", _emailAddress));
            emailMessage.To.Add(new MailboxAddress(firstName + " " + lastName, receiverEmail));
            emailMessage.Subject = "Confirmez votre inscription";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailBody };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                        _host,
                        _port,
                        MailKit.Security.SecureSocketOptions.StartTls
                    );
                    await client.AuthenticateAsync(_emailAddress, _password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task SendPasswordResetEmail(
            string receiverEmail,
            string firstName,
            string lastName,
            string resetLink
        )
        {
            var templateService = new EmailTemplateService();
            var emailBody = await templateService.GetPasswordResetEmailTemplate(
                firstName,
                lastName,
                resetLink,
                "VotreEntreprise"
            );

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("VotreEntreprise", _emailAddress));
            emailMessage.To.Add(new MailboxAddress(firstName + " " + lastName, receiverEmail));
            emailMessage.Subject = "Réinitialiser votre mot de passe";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailBody };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                        _host,
                        _port,
                        MailKit.Security.SecureSocketOptions.StartTls
                    );
                    await client.AuthenticateAsync(_emailAddress, _password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task Send2FAEmail(string receiverEmail, string firstName, string code)
        {
            var subject = "Votre code de vérification";
            var body =
                $"Bonjour {firstName},\n\nVoici votre code de vérification : {code}. Ce code expirera dans 15 minutes.\n\nMerci.";

            await SendMail(subject, body, receiverEmail, firstName);
        }
    }
}
