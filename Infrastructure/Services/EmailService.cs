using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public void SendEmail(string? subject, string? message, string? receiver)
        {
            if (string.IsNullOrWhiteSpace(receiver))
            {
                throw new ArgumentException("Receiver is null, empty or whitespace", nameof(receiver));
            }

            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = subject;
            mailMessage.To.Add(receiver);
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            _smtpClient.Send(mailMessage);
        }
    }
}
