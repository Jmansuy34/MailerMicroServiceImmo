using Mailer.DTOs;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Mailer.Service
{
    public class AgentService
    {
        private readonly MailerService _mailerService;
        private readonly string _jwtSecret;

        public AgentService(MailerService mailerService, IConfiguration configuration)
        {
            _mailerService = mailerService;
            _jwtSecret = configuration["JwtSettings:SecretKey"]; // Récupérer la clé secrète pour le JWT
        }

        public async Task RegisterAgent(AdminAgentDto agent, string authToken)
        {
            // Créer un token JWT avec les informations de l'agent et le authToken
            var token = GenerateJwtToken(agent, authToken, "registerAgent"); // Passer la route "registerAgent"

            // Générer le lien avec le token JWT
            var registrationLink = $"https://votresite.com/register?token={token}";

            var subject = "Complétez votre inscription";
            var body = $"Bonjour {agent.Name} {agent.LastName},\n\n"
                      + "Veuillez compléter votre inscription en cliquant sur le lien suivant :\n"
                      + $"{registrationLink}\n\n"
                      + "Merci.";

            // Envoyer l'email avec le lien d'inscription
            await _mailerService.SendMail(subject, body, agent.Email, agent.LastName);
        }

        // Générer un token JWT avec les informations de l'agent dans le payload, incluant authToken et route
        private string GenerateJwtToken(AdminAgentDto agent, string authToken, string route)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            // Créer les claims avec les données de l'agent, authToken et route
            var claims = new List<Claim>
            {
                new Claim("name", agent.Name),
                new Claim("lastName", agent.LastName),
                new Claim("email", agent.Email),
                new Claim("phone", agent.Phone),
                new Claim("agency", agent.Agency ?? string.Empty), // Agency peut être null
                new Claim("authToken", authToken), // Ajouter authToken dans les claims
                new Claim("route", route) // Ajouter la route dans les claims
            };

            // Créer la description du token avec expiration (exemple 1 heure)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Expiration du token (à ajuster selon le besoin)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
