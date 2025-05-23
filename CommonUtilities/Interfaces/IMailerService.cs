namespace CommonUtilities.Interfaces;

public interface IMailerService
{
    Task<bool> SendEmail(Mail mail);
}