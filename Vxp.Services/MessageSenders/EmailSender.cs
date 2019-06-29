using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace VRTX.Services.MessageSenders
{
    public class EmailSender : IEmailSender
    {
        private const string SenderName = "VERTEX DESIGH LTD";
        private const string SenderEmail = "noreply@vertexdesign.bg";
        private readonly string _sendGridApiKey;

        public EmailSender(IConfiguration configuration)
        {
            _sendGridApiKey = configuration["Authentication:SendGridApiKey"];
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(_sendGridApiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(SenderEmail, SenderName),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };

            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}