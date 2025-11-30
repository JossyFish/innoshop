using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Commands.ChangeProfile;
using UserService.Application.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ChangeProfileTests
    {
        Mock<IBaseUserService> _baseUserServiceMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        ChangeProfileHandler _handler;

        public ChangeProfileTests()
        {
            _baseUserServiceMock = new Mock<IBaseUserService>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _handler = new ChangeProfileHandler(_baseUserServiceMock.Object, _usersRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidNameChange_ShouldUpdateUserName()
        {
            var user = User.Create(Guid.NewGuid(), "Old Name", "test@test.com", "hashed_password", true, DateTime.UtcNow);
            var command = new ChangeProfileCommand { NewName = "New Name" };

            _baseUserServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _usersRepositoryMock.Setup(x => x.ChangeAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            user.Name.Should().Be("New Name");
            _usersRepositoryMock.Verify(x => x.ChangeAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
