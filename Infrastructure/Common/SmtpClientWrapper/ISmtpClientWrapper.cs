using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.SmtpClientWrapper
{
    public interface ISmtpClientWrapper
    {
        void Send(MailMessage message);
    }
}
