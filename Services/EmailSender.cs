using CarpooliDotTN.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Mail;

namespace CarpooliDotTN.Services
{

    public class EmailSender : IEmailSender
    {
        public void SendEmail(string toEmail, string subject, string body)
        {
            // Sender's email credentials
            string senderEmail = "ghassencherif22@gmail.com";
            string senderPassword = "";   //Add the email code for apps

            // Setup the MIME message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderEmail, senderEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                // Connect to the SMTP server using MailKit
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587);

                    // UseSecureSocketLayer should be true if you're using port 465 instead
                    client.Authenticate(senderEmail, senderPassword);

                    // Send the email
                    client.Send(message);

                    // Disconnect from the SMTP server
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or display an error message)
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}