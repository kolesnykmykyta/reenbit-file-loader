using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Common.SmtpClientWrapper;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services
{
    /// <summary>
    /// Provides methods for working with emails.
    /// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Contains SmtpClient, which is used to send emails.
        /// </summary>
        private readonly ISmtpClientWrapper _smtpClient;

        public EmailService(ISmtpClientWrapper smtpClient)
        {
            _smtpClient = smtpClient;
        }

        /// <summary>
        /// Sends email according to the arguments.
        /// </summary>
        /// <param name="receiver">Receiver of the email.</param>
        /// <param name="sender">Sender of the email.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="message">Email body.</param>
        /// <exception cref="ArgumentException">Thrown when the receiver or sender either null or invalid.</exception>
        public void SendEmail(string? receiver, string? sender, string? subject, string? message)
        {
            if (string.IsNullOrWhiteSpace(receiver))
            {
                throw new ArgumentException("Receiver email is null, empty or whitespace", nameof(receiver));
            }
            if (string.IsNullOrWhiteSpace(sender))
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
