using Auth.Core.Services.Interfaces;
using FluentAssertions;
using Moq;
using UserService.Application.Commands.ConfirmRegisterLink;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ConfirmRegisterLinkTests
    {
        Mock<ICacheUsersRepository> _cacheMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        Mock<IJwtProvider> _jwtProviderMock;
        Mock<ILinkTokenGenerator> _linkTokenGeneratorMock;
        ConfirmRegisterLinkHandler _handler;

        public ConfirmRegisterLinkTests()
        {
            _cacheMock = new Mock<ICacheUsersRepository>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _linkTokenGeneratorMock = new Mock<ILinkTokenGenerator>();
            _handler = new ConfirmRegisterLinkHandler(_cacheMock.Object, _usersRepositoryMock.Object, _jwtProviderMock.Object, _linkTokenGeneratorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidToken_ShouldCreateUserAndReturnToken()
        {
            var command = new ConfirmRegisterLinkCommand("valid_token");
            var email = "test@test.com";
            var creationUserData = new CreationUserData(
            Guid.NewGuid(), "Test User", email, "hashed_password", "123456", "valid_token");

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns(email);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            _cacheMock.Setup(x => x.GetRegistrationDataByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(creationUserData);
            _usersRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _cacheMock.Setup(x => x.RemoveRegistrationDataAsync(creationUserData.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _jwtProviderMock.Setup(x => x.GenerateToken(creationUserData.Id, Auth.Core.Enums.Role.User)).Returns("jwt_token");

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be("jwt_token");
            _usersRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(x => x.RemoveRegistrationDataAsync(creationUserData.Id, It.IsAny<CancellationToken>()), Times.Once);
            _jwtProviderMock.Verify(x => x.GenerateToken(creationUserData.Id, Auth.Core.Enums.Role.User), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidToken_ShouldThrowInvalidLinkTokenException()
        {
            var command = new ConfirmRegisterLinkCommand("invalid_token");

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns((string)null);

            await Assert.ThrowsAsync<InvalidLinkTokenException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserAlreadyExists_ShouldThrowUserAlreadyExistException()
        {
            var command = new ConfirmRegisterLinkCommand("valid_token");
            var email = "test@test.com";
            var existingUser = User.Create(Guid.NewGuid(), "Existing User", email, "hashed_password", true, DateTime.UtcNow);

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns(email);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<UserAlreadyExistException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CacheDataNotFound_ShouldThrowCacheDataNotFoundException()
        {
            var command = new ConfirmRegisterLinkCommand("valid_token");
            var email = "test@test.com";

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns(email);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            _cacheMock.Setup(x => x.GetRegistrationDataByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync((CreationUserData)null);

            await Assert.ThrowsAsync<CacheDataNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_TokenMismatch_ShouldThrowInvalidLinkTokenException()
        {
            var command = new ConfirmRegisterLinkCommand("different_token");
            var email = "test@test.com";
            var creationUserData = new CreationUserData(
                Guid.NewGuid(), "Test User", email, "hashed_password", "123456", "stored_token");

            _linkTokenGeneratorMock.Setup(x => x.ValidateToken(command.Token)).Returns(email);
            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            _cacheMock.Setup(x => x.GetRegistrationDataByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(creationUserData);

            await Assert.ThrowsAsync<InvalidLinkTokenException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
