using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Commands.ConfirmNewEmailCode;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ConfirmNewEmailCodeTests
    {
        Mock<IBaseUserService> _baseUserServiceMock;
        Mock<ICacheUsersRepository> _cacheMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        ConfirmNewEmailCodeHandler _handler;

        public ConfirmNewEmailCodeTests()
        {
            _baseUserServiceMock = new Mock<IBaseUserService>();
            _cacheMock = new Mock<ICacheUsersRepository>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _handler = new ConfirmNewEmailCodeHandler(_baseUserServiceMock.Object, _cacheMock.Object, _usersRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidConfirmationCode_ShouldUpdateEmail()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "old@test.com", "hashed_password", true, DateTime.UtcNow);
            var command = new ConfirmNewEmailCodeCommand { ConfirmationCode = "123456" };
            var changeEmailData = new ChangeEmailData(user.Id, "old@test.com", "new@test.com", "123456", "token123");

            _baseUserServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.GetChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(changeEmailData);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(changeEmailData.NewEmail, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            _usersRepositoryMock.Setup(x => x.ChangeAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _cacheMock.Setup(x => x.RemoveChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            user.Email.Should().Be("new@test.com");
            _cacheMock.Verify(x => x.RemoveChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidConfirmationCode_ShouldThrowInvalidConfirmCodeException()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "old@test.com", "hashed_password", true, DateTime.UtcNow);
            var command = new ConfirmNewEmailCodeCommand { ConfirmationCode = "wrong_code" };
            var changeEmailData = new ChangeEmailData(user.Id, "old@test.com", "new@test.com", "123456", "token123");

            _baseUserServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.GetChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(changeEmailData);

            await Assert.ThrowsAsync<InvalidConfirmCodeException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
