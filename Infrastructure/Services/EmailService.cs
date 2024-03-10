using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public void SendEmail(string? receiver, string? sender, string? subject, string? message)
        {
            if (string.IsNullOrWhiteSpace(receiver))
            {
                throw new ArgumentException("Receiver email is null, empty or whitespace", nameof(receiver));
            }
            if (string.IsNullOrEmpty(sender))
            {
                throw new ArgumentException("Sender email is null, empty or whitespace", nameof(sender));
            }

            MailMessage mailMessage = new MailMessage();

            try
            {
                mailMessage.From = new MailAddress(sender!);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Sender email is not a valid email address", nameof(sender));
            }

            mailMessage.Subject = subject;
            mailMessage.From = new MailAddress(sender!);
            mailMessage.To.Add(receiver);
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            _smtpClient.Send(mailMessage);
        }
    }
}
