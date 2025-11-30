using Auth.Core.Services.Interfaces;
using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;
using UserService.Application.Commands.Delete;
using UserService.Application.Events;
using UserService.Domain.Exceptions;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Commands
{
    public class DeleteTests
    {
        Mock<IUserContext> _userContextMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        Mock<IPublishEndpoint> _publishEndpointMock;
        DeleteHandler _handler;

        public DeleteTests()
        {
            _userContextMock = new Mock<IUserContext>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _handler = new DeleteHandler(_userContextMock.Object, _usersRepositoryMock.Object, _publishEndpointMock.Object);
        }

        [Fact]
        public async Task Handle_ValidUser_ShouldDeleteAndPublishEvent()
        {
            var userId = Guid.NewGuid();
            var command = new DeleteCommand();

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _usersRepositoryMock.Setup(x => x.DeleteByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _publishEndpointMock.Setup(x => x.Publish(It.IsAny<UserDeletedEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            _publishEndpointMock.Verify(x => x.Publish(It.Is<UserDeletedEvent>(e => e.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowUserNotFoundByIdException()
        {
            var userId = Guid.NewGuid();
            var command = new DeleteCommand();

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _usersRepositoryMock.Setup(x => x.DeleteByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<UserNotFoundByIdException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
