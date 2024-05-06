using System.ComponentModel.DataAnnotations;

namespace Mailer.DTOs
{
    public class EmailRequest
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MinLength(1)]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [MinLength(1)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Une adresse email valide est obligatoire.")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Le sujet est obligatoire.")]
        [MinLength(1)]
        public required string Subject { get; set; }

        [Required(ErrorMessage = "La description est obligatoire.")]
        [MinLength(1)]
        public required string Description { get; set; }
    }
}
