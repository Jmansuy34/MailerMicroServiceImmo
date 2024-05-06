namespace Mailer.Interface
{
    public interface IMailerService
    {
        Task SendMail(string subject, string text, string receiverMail, string lastName);
    }
}
