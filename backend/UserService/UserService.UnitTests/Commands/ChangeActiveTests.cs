using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;
using UserService.Application.Commands.ChangeActive;
using UserService.Application.Events;
using UserService.Application.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class ChangeActiveTests
    {
        Mock<IBaseUserService> _baseUserServiceMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        Mock<IPublishEndpoint> _publishEndpointMock;
        ChangeActiveHandler _handler;

        public ChangeActiveTests()
        {
            _baseUserServiceMock = new Mock<IBaseUserService>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _handler = new ChangeActiveHandler(_baseUserServiceMock.Object, _usersRepositoryMock.Object, _publishEndpointMock.Object);
        }

        [Fact]
        public async Task Handle_DeactivateUser_ShouldPublishUserDeactivatedEvent()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "test@test.com", "hashed_password", true, DateTime.UtcNow);
            var command = new ChangeActiveCommand { IsActive = false };

            _baseUserServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _usersRepositoryMock.Setup(x => x.ChangeAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            user.IsActive.Should().BeFalse();
            _publishEndpointMock.Verify(x => x.Publish(It.Is<UserDeactivatedEvent>(e => e.UserId == user.Id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ActivateUser_ShouldNotPublishEvent()
        {
            var user = User.Create(Guid.NewGuid(), "Test User", "test@test.com", "hashed_password", false, DateTime.UtcNow);
            var command = new ChangeActiveCommand { IsActive = true };

            _baseUserServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _usersRepositoryMock.Setup(x => x.ChangeAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            user.IsActive.Should().BeTrue();
            _publishEndpointMock.Verify(x => x.Publish(It.IsAny<UserDeactivatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
