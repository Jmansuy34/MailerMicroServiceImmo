using System.ComponentModel.DataAnnotations;

namespace Mailer.DTOs
{
    public class AdminAgentDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        public string? Agency { get; set; } 
        [Required]
        public string AuthToken { get; set; } 
    }
}
