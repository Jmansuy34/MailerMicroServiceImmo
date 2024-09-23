using System.IO;
using System.Threading.Tasks;

public class EmailTemplateService
{
    public async Task<string> GetRegistrationEmailTemplate(string firstName, string lastName, string urlLink, string companyName)
    {
        // Charger le fichier template
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "RegistrationTemplate.html");
        var templateContent = await File.ReadAllTextAsync(templatePath);

        // Remplacer les placeholders par les données réelles
        var emailContent = templateContent
            .Replace("{{FirstName}}", firstName)
            .Replace("{{LastName}}", lastName)
            .Replace("{{urlLink}}", urlLink)
            .Replace("{{CompanyName}}", companyName);

        return emailContent;
    }

    
     // Nouvelle méthode pour le template de réinitialisation de mot de passe
    public async Task<string> GetPasswordResetEmailTemplate(string firstName, string lastName, string resetLink, string companyName)
    {
        // Chemin vers le template HTML de réinitialisation de mot de passe
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "PasswordResetTemplate.html");
        var templateContent = await File.ReadAllTextAsync(templatePath);

        // Remplacer les placeholders dans le template avec les données dynamiques
        var emailContent = templateContent
            .Replace("{{FirstName}}", firstName)
            .Replace("{{LastName}}", lastName)
            .Replace("{{ResetLink}}", resetLink)
            .Replace("{{CompanyName}}", companyName);

        return emailContent;
    }

    public async Task<string> GetSimpleEmailTemplate(string lastName, string firstName, string email, string description)
        {
            // Chemin vers le template HTML
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "SimpleEmailTemplate.html");
            var templateContent = await File.ReadAllTextAsync(templatePath);

            // Remplacer les placeholders par les valeurs dynamiques
            var emailContent = templateContent
                .Replace("{{LastName}}", lastName)
                .Replace("{{FirstName}}", firstName)
                .Replace("{{Email}}", email)
                .Replace("{{Description}}", description);

            return emailContent;
        }
}
