using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Commands.ResetPassword;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ResetPasswordTests
    {
        Mock<ICacheUsersRepository> _cacheMock;
        Mock<IConfirmCodeGenerator> _codeGeneratorMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        Mock<IEmailService> _emailServiceMock;
        ResetPasswordHandler _handler;

        public ResetPasswordTests()
        {
            _cacheMock = new Mock<ICacheUsersRepository>();
            _codeGeneratorMock = new Mock<IConfirmCodeGenerator>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _handler = new ResetPasswordHandler(_cacheMock.Object, _codeGeneratorMock.Object, _usersRepositoryMock.Object, _emailServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidEmail_ShouldSaveResetDataAndSendEmail()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "test@test.com", "hashed_password", true, DateTime.UtcNow);
            var command = new ResetPasswordCommand { Email = "test@test.com" };

            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.RemoveResetPasswordDataAsync(user.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _codeGeneratorMock.Setup(x => x.Generate()).Returns("123456");
            _cacheMock.Setup(x => x.SaveResetPasswordDataAsync(It.IsAny<ResetPasswordData>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _emailServiceMock.Setup(x => x.SendResetPasswordAsync(user.Email, "123456", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            _cacheMock.Verify(x => x.SaveResetPasswordDataAsync(
                It.Is<ResetPasswordData>(d =>
                    d.Id == user.Id &&
                    d.Email == user.Email &&
                    d.ConfirmationCode == "123456"),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
