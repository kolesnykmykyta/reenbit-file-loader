using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.SmtpClientWrapper
{
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

        public void Send(MailMessage message)
        {
            _smtpClient.Send(message);
        }
    }
}
