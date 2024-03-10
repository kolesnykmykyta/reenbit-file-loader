using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.SmtpClientWrapper
{
    /// <summary>
    /// Wraps SmtpClient to make its usage in DI containers and unit tests possible.
    /// </summary>
    public class SmtpClientWrapper : ISmtpClientWrapper
    {
        private readonly SmtpClient _smtpClient;

        public SmtpClientWrapper(int port, NetworkCredential credential, bool enableSsl)
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = port,
                Credentials = credential,
                EnableSsl = enableSsl,
            };
        }

        /// <summary>
        /// Invokes wrapped SmtpClient method to send the message.
        /// </summary>
        /// <param name="message">Email message.</param>
        public void Send(MailMessage message)
        {
            _smtpClient.Send(message);
        }
    }
}
