using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using UserService.Application.Commands.CreateUser;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class CreateUserTests
    {
        Mock<ICacheUsersRepository> _cacheMock;
        Mock<IPasswordHasher> _passwordHasherMock;
        Mock<IConfirmCodeGenerator> _codeGeneratorMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        Mock<IEmailService> _emailServiceMock;
        Mock<ILinkTokenGenerator> _tokenGeneratorMock;
        CreateUserHandler _handler;
        AppSettings _appSettings;

        public CreateUserTests()
        {
            _cacheMock = new Mock<ICacheUsersRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _codeGeneratorMock = new Mock<IConfirmCodeGenerator>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _tokenGeneratorMock = new Mock<ILinkTokenGenerator>();

            _appSettings = new AppSettings { BaseUrl = "https://test.com" };
            var options = Options.Create(_appSettings);

            _handler = new CreateUserHandler(
                _cacheMock.Object,
                _passwordHasherMock.Object,
                _codeGeneratorMock.Object,
                _usersRepositoryMock.Object,
                _emailServiceMock.Object,
                _tokenGeneratorMock.Object,
                options);
        }

        [Fact]
        public async Task Handle_NewUser_ShouldSaveToCacheAndSendEmail()
        {
            var command = new CreateUserCommand
            {
                Name = "Test User",
                Email = "test@test.com",
                Password = "password123"
            };

            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            _cacheMock.Setup(x => x.RemoveRegistrationDataByEmailAsync(command.Email, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _passwordHasherMock.Setup(x => x.Generate(command.Password)).Returns("hashed_password");
            _codeGeneratorMock.Setup(x => x.Generate()).Returns("123456");
            _tokenGeneratorMock.Setup(x => x.GenerateToken(command.Email)).Returns("token123");
            _cacheMock.Setup(x => x.SaveRegistrationDataAsync(It.IsAny<CreationUserData>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(command.Email, "123456", It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            _cacheMock.Verify(x => x.SaveRegistrationDataAsync(It.IsAny<CreationUserData>(), It.IsAny<CancellationToken>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(command.Email, "123456", It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
