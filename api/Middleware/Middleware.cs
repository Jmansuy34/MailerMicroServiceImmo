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
            var token = context
                .Request.Headers["Authorization"]
                .FirstOrDefault()
                ?.Split(" ")
                .Last();
            if (!string.IsNullOrEmpty(token))
            {
                context.Items["AuthToken"] = token;
                var email = UncodeJwtToken(token);
                if (!string.IsNullOrEmpty(email))
                {
                    context.Items["Email"] = email;
                }
            }
            await _next(context);
        }

        public string UncodeJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var uncodeToken = handler.ReadToken(token) as JwtSecurityToken;
            var email = uncodeToken.Claims.First(claim => claim.Type == "email").Value;
            return email;
        }
    }
}
