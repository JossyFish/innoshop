using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Commands.ConfirmNewPassword;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ConfirmNewPasswordTests
    {
        Mock<ICacheUsersRepository> _cacheMock;
        Mock<IPasswordHasher> _passwordHasherMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        ConfirmNewPasswordHandler _handler;

        public ConfirmNewPasswordTests()
        {
            _cacheMock = new Mock<ICacheUsersRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _handler = new ConfirmNewPasswordHandler(_cacheMock.Object, _passwordHasherMock.Object, _usersRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidResetPassword_ShouldUpdatePassword()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "test@test.com", "old_hashed_password", true, DateTime.UtcNow);
            var command = new ConfirmNewPasswordCommand
            {
                Email = "test@test.com",
                NewPassword = "new123",
                ConfirmationCode = "123456"
            };
            var resetPasswordData = new ResetPasswordData(user.Id, "test@test.com", "123456");

            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.GetResetPasswordDataAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(resetPasswordData);
            _cacheMock.Setup(x => x.RemoveResetPasswordDataAsync(user.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _passwordHasherMock.Setup(x => x.Generate(command.NewPassword)).Returns("new_hashed_password");
            _usersRepositoryMock.Setup(x => x.ChangeAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            user.PasswordHash.Should().Be("new_hashed_password");
            _cacheMock.Verify(x => x.RemoveResetPasswordDataAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
