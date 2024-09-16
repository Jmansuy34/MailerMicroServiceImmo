using System.IdentityModel.Tokens.Jwt;

namespace Mailer.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Extraire le jeton JWT depuis l'en-tête Authorization
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Si un jeton est présent, extraire l'email du jeton
            if (!string.IsNullOrEmpty(token))
            {
                context.Items["AuthToken"] = token;
                var email = UncodeJwtToken(token);

                // Si l'email est trouvé dans le jeton, l'ajouter au contexte
                if (!string.IsNullOrEmpty(email))
                {
                    context.Items["Email"] = email;
                }
            }

            await _next(context);
        }

        // Fonction pour décoder le jeton JWT et extraire l'email
        public string UncodeJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadToken(token) as JwtSecurityToken;

            // Récupérer la revendication (claim) "email" dans le jeton
            var email = decodedToken?.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

            return email;
        }
    }
}
