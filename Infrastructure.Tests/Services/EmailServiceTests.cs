using Infrastructure.Common.SmtpClientWrapper;
using Infrastructure.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Services
{
    public class EmailServiceTests
    {
        [Fact]
        public void SendEmail_CorrectValues_SendsEmail()
        {
            var smtpMock = new Mock<ISmtpClientWrapper>();
            EmailService service = new EmailService(smtpMock.Object);

            service.SendEmail("valid@email", "valid@email", "test", "test");

            smtpMock.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("not-email")]
        public void SendEmail_InvalidSender_ThrowsArgumentException(string? sender)
        {
            EmailService service = new EmailService(new Mock<ISmtpClientWrapper>().Object);
            string expectedParamName = "sender";

            var exception = Assert.Throws<ArgumentException>(() => service.SendEmail("valid@email", sender, "test", "test"));
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void SendEmail_InvalidReceiver_ThrowsArgumentException(string? receiver)
        {
            EmailService service = new EmailService(new Mock<ISmtpClientWrapper>().Object);
            string expectedParamName = "receiver";

            var exception = Assert.Throws<ArgumentException>(() => service.SendEmail(receiver, "valid@email", "test", "test"));
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
        }
    }
}
