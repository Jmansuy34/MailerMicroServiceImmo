using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace TestMailer.Service
{
    public class TestMailerService
    {
        private readonly string _emailAddress;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;

        public TestMailerService(IConfiguration configuration)
        {
            _emailAddress = configuration["EmailSettings:EmailAddress"];
            _password = configuration["EmailSettings:Password"];
            _host = configuration["EmailSettings:Host"];
            _port = int.Parse(configuration["EmailSettings:Port"]);
            _enableSsl = bool.Parse(configuration["EmailSettings:EnableSSL"]);
        }

        public virtual async Task SendMail(
            string subject,
            string text,
            string receiverMail,
            string lastName
        )
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("IMMO-IA", _emailAddress));
            emailMessage.To.Add(new MailboxAddress(lastName, receiverMail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = text };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                        _host,
                        _port,
                        _enableSsl
                            ? MailKit.Security.SecureSocketOptions.SslOnConnect
                            : MailKit.Security.SecureSocketOptions.StartTls
                    );
                    await client.AuthenticateAsync(_emailAddress, _password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (SmtpCommandException ex)
                {
                    Console.WriteLine(
                        $"Erreur dans l'envoi de l'email: {ex.Message}, Status Code: {ex.StatusCode}"
                    );
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
