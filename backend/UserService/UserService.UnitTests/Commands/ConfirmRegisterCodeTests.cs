using Auth.Core.Services.Interfaces;
using FluentAssertions;
using Moq;
using UserService.Application.Commands.ConfirmRegisterCode;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ConfirmRegisterCodeTests
    {
        Mock<ICacheUsersRepository> _cacheMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        Mock<IJwtProvider> _jwtProviderMock;
        ConfirmRegisterCodeHandler _handler;

        public ConfirmRegisterCodeTests()
        {
            _cacheMock = new Mock<ICacheUsersRepository>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _handler = new ConfirmRegisterCodeHandler(_cacheMock.Object, _usersRepositoryMock.Object, _jwtProviderMock.Object);
        }

        [Fact]
        public async Task Handle_InvalidConfirmationCode_ShouldThrowInvalidConfirmCodeException()
        {
            var command = new ConfirmRegisterCodeCommand
            {
                Email = "test@test.com",
                ConfirmationCode = "wrong_code"
            };
            var creationUserData = new CreationUserData(
                Guid.NewGuid(), "Test User", "test@test.com", "hashed_password", "123456", "token123");

            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _cacheMock.Setup(x => x.GetRegistrationDataByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(creationUserData);

            await Assert.ThrowsAsync<InvalidConfirmCodeException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CacheDataNotFound_ShouldThrowCacheDataNotFoundException()
        {
            var command = new ConfirmRegisterCodeCommand
            {
                Email = "test@test.com",
                ConfirmationCode = "123456"
            };

            _usersRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _cacheMock.Setup(x => x.GetRegistrationDataByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreationUserData)null);

            await Assert.ThrowsAsync<CacheDataNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
