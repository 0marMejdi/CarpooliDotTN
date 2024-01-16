using CarpooliDotTN.Models;

namespace CarpooliDotTN.Services
{
    public interface IEmailSender
    {
        void SendEmail(string toEmail, string subject, string body);
    }
}
