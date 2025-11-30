using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Commands.ConfirmNewEmailLink;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ConfirmNewEmailLinkTests
    {
        Mock<ICacheUsersRepository> _cacheMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        Mock<ILinkTokenGenerator> _linkTokenGeneratorMock;
        ConfirmNewEmailLinkHandler _handler;

        public ConfirmNewEmailLinkTests()
        {
            _cacheMock = new Mock<ICacheUsersRepository>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _linkTokenGeneratorMock = new Mock<ILinkTokenGenerator>();
            _handler = new ConfirmNewEmailLinkHandler(_cacheMock.Object, _usersRepositoryMock.Object, _linkTokenGeneratorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidToken_ShouldUpdateUserEmail()
        {
            var command = new ConfirmNewEmailLinkCommand("valid_token");
            var userEmail = "old@test.com";
            var user = User.Create(Guid.NewGuid(), "Test User", userEmail, "hashed_password", true, DateTime.UtcNow);
            var changeEmailData = new ChangeEmailData(user.Id, userEmail, "new@test.com", "123456", "valid_token");

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns(userEmail);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.GetChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(changeEmailData);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(changeEmailData.NewEmail, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            _usersRepositoryMock.Setup(x => x.ChangeAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _cacheMock.Setup(x => x.RemoveChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            user.Email.Should().Be("new@test.com");
            _usersRepositoryMock.Verify(x => x.ChangeAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(x => x.RemoveChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidToken_ShouldThrowInvalidLinkTokenException()
        {
            var command = new ConfirmNewEmailLinkCommand("invalid_token");

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns((string)null);

            await Assert.ThrowsAsync<InvalidLinkTokenException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowUserNotFoundException()
        {
            var command = new ConfirmNewEmailLinkCommand("valid_token");
            var userEmail = "nonexistent@test.com";

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns(userEmail);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<UserNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CacheDataNotFound_ShouldThrowCacheDataNotFoundException()
        {
            var command = new ConfirmNewEmailLinkCommand("valid_token");
            var userEmail = "test@test.com";
            var user = User.Create(Guid.NewGuid(), "Test User", userEmail, "hashed_password", true, DateTime.UtcNow);

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns(userEmail);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.GetChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync((ChangeEmailData)null);

            await Assert.ThrowsAsync<CacheDataNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_TokenMismatch_ShouldThrowInvalidLinkTokenException()
        {
            var command = new ConfirmNewEmailLinkCommand("different_token");
            var userEmail = "test@test.com";
            var user = User.Create(Guid.NewGuid(), "Test User", userEmail, "hashed_password", true, DateTime.UtcNow);
            var changeEmailData = new ChangeEmailData(user.Id, userEmail, "new@test.com", "123456", "stored_token");

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns(userEmail);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.GetChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(changeEmailData);

            await Assert.ThrowsAsync<InvalidLinkTokenException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NewEmailAlreadyExists_ShouldThrowUserAlreadyExistException()
        {
            var command = new ConfirmNewEmailLinkCommand("valid_token");
            var userEmail = "old@test.com";
            var user = User.Create(Guid.NewGuid(), "Test User", userEmail, "hashed_password", true, DateTime.UtcNow);
            var changeEmailData = new ChangeEmailData(user.Id, userEmail, "new@test.com", "123456", "valid_token");
            var existingUser = User.Create(Guid.NewGuid(), "Existing User", "new@test.com", "hashed_password", true, DateTime.UtcNow);

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns(userEmail);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _cacheMock.Setup(x => x.GetChangeEmailDataAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(changeEmailData);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(changeEmailData.NewEmail, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<UserAlreadyExistException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
