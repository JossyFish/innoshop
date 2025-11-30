using Auth.Core.Services.Interfaces;
using FluentAssertions;
using Moq;
using UserService.Application.Commands.Login;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class LoginTests
    {
        Mock<IUsersRepository> _usersRepositoryMock;
        Mock<IPasswordHasher> _passwordHasherMock;
        Mock<IJwtProvider> _jwtProviderMock;
        LoginHandler _handler;

        public LoginTests()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _handler = new LoginHandler(_usersRepositoryMock.Object, _passwordHasherMock.Object, _jwtProviderMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ShouldReturnToken()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "test@test.com", "hashed_password", true, DateTime.UtcNow);
            var command = new LoginCommand { Email = "test@test.com", Password = "password123" };

            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.Verify(command.Password, user.PasswordHash)).Returns(true);
            _jwtProviderMock.Setup(x => x.GenerateToken(user.Id, It.IsAny<Auth.Core.Enums.Role[]>())).Returns("jwt_token");

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be("jwt_token");
            _jwtProviderMock.Verify(x => x.GenerateToken(user.Id, It.IsAny<Auth.Core.Enums.Role[]>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidPassword_ShouldThrowInvalidPasswordException()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "test@test.com", "hashed_password", true, DateTime.UtcNow);
            var command = new LoginCommand { Email = "test@test.com", Password = "wrong_password" };

            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.Verify(command.Password, user.PasswordHash)).Returns(false);

            await Assert.ThrowsAsync<InvalidPasswordException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
