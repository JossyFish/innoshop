using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Commands.ChangePassword;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ChangePasswordTests
    {
        Mock<IBaseUserService> _baseUserServiceMock;
        Mock<IPasswordHasher> _passwordHasherMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        ChangePasswordHandler _handler;

        public ChangePasswordTests()
        {
            _baseUserServiceMock = new Mock<IBaseUserService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _handler = new ChangePasswordHandler(_baseUserServiceMock.Object, _passwordHasherMock.Object, _usersRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidPasswordChange_ShouldUpdatePassword()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "test@test.com", "old_hashed_password", true, DateTime.UtcNow);
            var command = new ChangePasswordCommand
            {
                CurrentPassword = "current123",
                NewPassword = "new123"
            };

            _baseUserServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.Verify(command.CurrentPassword, user.PasswordHash)).Returns(true);
            _passwordHasherMock.Setup(x => x.Generate(command.NewPassword)).Returns("new_hashed_password");
            _usersRepositoryMock.Setup(x => x.ChangeAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            user.PasswordHash.Should().Be("new_hashed_password");
            _usersRepositoryMock.Verify(x => x.ChangeAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCurrentPassword_ShouldThrowInvalidPasswordException()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "test@test.com", "old_hashed_password", true, DateTime.UtcNow);
            var command = new ChangePasswordCommand
            {
                CurrentPassword = "wrong_password",
                NewPassword = "new123"
            };

            _baseUserServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.Verify(command.CurrentPassword, user.PasswordHash)).Returns(false);

            await Assert.ThrowsAsync<InvalidPasswordException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
