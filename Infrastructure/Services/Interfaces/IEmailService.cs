using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(string? receiver, string? sender, string? subject, string? message);
    }
}
