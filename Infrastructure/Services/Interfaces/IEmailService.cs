using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(string? subject, string? message, string? receiver);
    }
}
