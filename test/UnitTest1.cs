using System.Threading.Tasks;
using Mailer.DTOs;
using Mailer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; // Assurez-vous que cet espace de noms est inclus pour HttpContext
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestMailer.Controllers;
using Xunit;

namespace xUnitMailer.Tests
{
    public class MailControllerTests
    {
        private readonly Mock<IMailerService> _mailerServiceMock;
        private readonly MailController _controller;
        private readonly DefaultHttpContext _httpContext;

        public MailControllerTests()
        {
            _mailerServiceMock = new Mock<IMailerService>();
            _httpContext = new DefaultHttpContext();

            _controller = new MailController(_mailerServiceMock.Object)
            {
                ControllerContext = new ControllerContext() { HttpContext = _httpContext }
            };
        }

        [Fact]
        public async Task SendMail_ReturnsBadRequest_WhenEmailIsEmpty()
        {
            _httpContext.Items["Email"] = "";

            var emailRequest = new EmailRequest
            {
                LastName = "Doe",
                FirstName = "John",
                Email = "john@example.com",
                Subject = "Test Subject",
                Description = "Test Description"
            };

            var result = await _controller.SendMail(emailRequest);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SendMail_ReturnsOk_WhenEmailIsSentSuccessfully()
        {
            _httpContext.Items["Email"] = "test@example.com";
            _mailerServiceMock
                .Setup(m =>
                    m.SendMail(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        "test@example.com",
                        It.IsAny<string>()
                    )
                )
                .Returns(Task.CompletedTask);

            var emailRequest = new EmailRequest
            {
                LastName = "Doe",
                FirstName = "John",
                Email = "john.doe@example.com",
                Subject = "Test Subject",
                Description = "Test Description"
            };

            var result = await _controller.SendMail(emailRequest);

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async Task SendMail_ReturnsServerError_WhenExceptionIsThrown()
        {
            _httpContext.Items["Email"] = "john.doe@example.com";

            _mailerServiceMock
                .Setup(m =>
                    m.SendMail(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        "john.doe@example.com",
                        It.IsAny<string>()
                    )
                )
                .Throws(new Exception("Test exception"));

            var emailRequest = new EmailRequest
            {
                LastName = "Doe",
                FirstName = "John",
                Email = "john.doe@example.com",
                Subject = "Test Subject",
                Description = "Test Description"
            };

            var result = await _controller.SendMail(emailRequest);

            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }
    }
}
