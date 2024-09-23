using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace Mailer.Service
{
    public class TwoFactorAuthService
    {
        private readonly IMemoryCache _cache;
        private readonly int _maxAttempts = 3; // Nombre maximal de tentatives autorisées

        public TwoFactorAuthService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateAndStore2FACode(string email)
        {
            string code = Generate2FACode();
            string hashedCode = HashCode(code);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(15)); // Expiration après 15 minutes

            _cache.Set(email, new TwoFactorCacheEntry(hashedCode, _maxAttempts), cacheEntryOptions); // Associe l'email avec le hash du code et les tentatives restantes

            return code; 
        }

        private string Generate2FACode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string HashCode(string code)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(code));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool Validate2FACode(string email, string inputCode)
        {
            if (_cache.TryGetValue(email, out TwoFactorCacheEntry entry))
            {
                if (entry.AttemptsRemaining <= 0)
                {
                    return false; // Plus de tentatives disponibles
                }

                string hashedInputCode = HashCode(inputCode);
                if (hashedInputCode == entry.HashedCode)
                {
                    return true; // Code valide
                }
                else
                {
                    entry.AttemptsRemaining--;
                    _cache.Set(email, entry); // Met à jour les tentatives restantes
                    return false; // Code invalide
                }
            }

            return false; // Code expiré ou non trouvé
        }
    }

    // Modèle pour stocker le hash et le nombre de tentatives dans le cache
    public class TwoFactorCacheEntry
    {
        public string HashedCode { get; }
        public int AttemptsRemaining { get; set; }

        public TwoFactorCacheEntry(string hashedCode, int attemptsRemaining)
        {
            HashedCode = hashedCode;
            AttemptsRemaining = attemptsRemaining;
        }
    }
}
