using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace Mailer.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration; 

        public TokenMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Extraire le jeton JWT depuis l'en-tête Authorization
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Si un jeton est présent, ajouter l'adresse email depuis la configuration
            if (!string.IsNullOrEmpty(token))
            {
                context.Items["AuthToken"] = token;

                // Récupérer l'adresse email depuis la configuration
                var email = _configuration["EmailSettings:EmailAddress"];

                // Ajouter l'email récupéré au contexte
                if (!string.IsNullOrEmpty(email))
                {
                    context.Items["Email"] = email;
                }
            }

            await _next(context);
        }
    }
}
