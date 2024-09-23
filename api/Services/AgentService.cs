using Mailer.DTOs;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Mailer.Service
{
    public class AgentService
    {
        private readonly MailerService _mailerService;
        private readonly EmailTemplateService _emailTemplateService; // Injection du service de template
        private readonly string _jwtSecret;
        private readonly string _companyName;

        public AgentService(MailerService mailerService, EmailTemplateService emailTemplateService, IConfiguration configuration)
        {
            _mailerService = mailerService;
            _emailTemplateService = emailTemplateService; // Initialiser le service de template
            _jwtSecret = configuration["JwtSettings:SecretKey"];
            _companyName = configuration["CompanySettings:Name"];// Récupérer le nom de l'entreprise
        }

        public async Task RegisterAgent(AdminAgentDto agent, string authToken)
    {
        var token = GenerateJwtToken(agent, authToken, "registerAgent");
        var registrationLink = $"https://votresite.com/register?token={token}";

        // Charger le template d'email
        var emailBody = await _emailTemplateService.GetRegistrationEmailTemplate(
            agent.Name,
            agent.LastName,
            registrationLink,
            _companyName 
        );

        var subject = "Complétez votre inscription";
        await _mailerService.SendMail(subject, emailBody, agent.Email, agent.LastName);
    }

        // Générer un token JWT
        private string GenerateJwtToken(AdminAgentDto agent, string authToken, string route)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            // Créer les claims avec les données de l'agent
            var claims = new List<Claim>
            {
                new Claim("name", agent.Name),
                new Claim("lastName", agent.LastName),
                new Claim("email", agent.Email),
                new Claim("phone", agent.Phone),
                new Claim("agency", agent.Agency ?? string.Empty),
                new Claim("authToken", authToken),
                new Claim("route", route)
            };

            // Créer le token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
