using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using UserService.Application.Commands.ChangeEmail;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ChangeEmailTests
    {
        Mock<IBaseUserService> _baseUserServiceMock;
        Mock<ICacheUsersRepository> _cacheMock;
        Mock<IConfirmCodeGenerator> _codeGeneratorMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        Mock<IEmailService> _emailServiceMock;
        Mock<ILinkTokenGenerator> _tokenGeneratorMock;
        ChangeEmailHandler _handler;
        AppSettings _appSettings;

        public ChangeEmailTests()
        {
            _baseUserServiceMock = new Mock<IBaseUserService>();
            _cacheMock = new Mock<ICacheUsersRepository>();
            _codeGeneratorMock = new Mock<IConfirmCodeGenerator>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _tokenGeneratorMock = new Mock<ILinkTokenGenerator>();

            _appSettings = new AppSettings { BaseUrl = "https://test.com" };
            var options = Options.Create(_appSettings);

            _handler = new ChangeEmailHandler(
                _baseUserServiceMock.Object,
                _cacheMock.Object,
                _codeGeneratorMock.Object,
                _usersRepositoryMock.Object,
                _emailServiceMock.Object,
                _tokenGeneratorMock.Object,
                options);
        }

        [Fact]
        public async Task Handle_ValidNewEmail_ShouldSaveToCacheAndSendEmail()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "old@test.com", "hashed_password", true, DateTime.UtcNow);
            var command = new ChangeEmailCommand { NewEmail = "new@test.com" };

            _baseUserServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.RemoveChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(command.NewEmail, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            _codeGeneratorMock.Setup(x => x.Generate()).Returns("123456");
            _tokenGeneratorMock.Setup(x => x.GenerateToken(user.Email)).Returns("token123");
            _cacheMock.Setup(x => x.SaveChangeEmailDataAsync(It.IsAny<ChangeEmailData>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(command.NewEmail, "123456", It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            _cacheMock.Verify(x => x.SaveChangeEmailDataAsync(
                It.Is<ChangeEmailData>(d =>
                    d.Id == user.Id &&
                    d.OldEmail == user.Email &&
                    d.NewEmail == command.NewEmail &&
                    d.ConfirmationCode == "123456" &&
                    d.LinkToken == "token123"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmailAlreadyExists_ShouldThrowUserAlreadyExistException()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "old@test.com", "hashed_password", true, DateTime.UtcNow);
            var existingUser = User.Create(Guid.NewGuid(), "Existing User", "new@test.com", "hashed_password", true, DateTime.UtcNow);
            var command = new ChangeEmailCommand { NewEmail = "new@test.com" };

            _baseUserServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.RemoveChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(command.NewEmail, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<UserAlreadyExistException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
