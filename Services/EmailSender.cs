using CarpooliDotTN.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CarpooliDotTN.Services
{

    public class EmailSender : IEmailSender
    {
        public void SendEmail(string toEmail, string subject, string body)
        {
            // Sender's email credentials
            string senderEmail = "janick.conn26@ethereal.email";
            string senderPassword = "Qbr7pvwQTYCM15R4MQ";

            // Setup the MIME message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("janick.conn26@ethereal.email", senderEmail));
            message.To.Add(new MailboxAddress("janick.conn26@ethereal.email", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                // Connect to the SMTP server using MailKit
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);

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
