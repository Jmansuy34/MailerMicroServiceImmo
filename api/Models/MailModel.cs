namespace Mailer.Models
{
    public class EmailRequestModel
    {
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public required string Email { get; set; }
        public required string Subject { get; set; }
        public required string Description { get; set; }
    }
}
