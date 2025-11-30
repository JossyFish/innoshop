using FluentAssertions;
using Moq;
using UserService.Application.Interfaces;
using UserService.Application.Queries.GetCurrentUser;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Queries
{
    public class GetCurrentUserTests
    {
        Mock<IBaseUserService> _baseUserServiceMock;
        Mock<IUsersRepository> _usersRepositoryMock;
        GetCurrentUserHandler _handler;

        public GetCurrentUserTests()
        {
            _baseUserServiceMock = new Mock<IBaseUserService>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _handler = new GetCurrentUserHandler(_baseUserServiceMock.Object, _usersRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidUser_ShouldReturnCurrentUserResponse()
        {
            var user = User.Create(
                Guid.NewGuid(),
                "Test User",
                "test@test.com",
                "hashed_password",
                true,
                DateTime.UtcNow
            );

            var query = new GetCurrentUserQuery();

            _baseUserServiceMock
                .Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Name.Should().Be(user.Name);
            result.Email.Should().Be(user.Email);
            result.IsActive.Should().Be(user.IsActive);
            result.CreatedAt.Should().Be(user.CreatedAt);
            result.Roles.Should().BeEquivalentTo(user.Roles);

            _baseUserServiceMock.Verify(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InactiveUser_ShouldReturnCorrectData()
        {
            var user = User.Create(
                Guid.NewGuid(),
                "Inactive User",
                "inactive@test.com",
                "hashed_password",
                false,
                DateTime.UtcNow.AddDays(-30)
            );

            var query = new GetCurrentUserQuery();

            _baseUserServiceMock
                .Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Name.Should().Be("Inactive User");
            result.Email.Should().Be("inactive@test.com");
            result.IsActive.Should().BeFalse();
            result.CreatedAt.Should().Be(user.CreatedAt);
        }

        [Fact]
        public async Task Handle_UserWithRoles_ShouldReturnRolesCorrectly()
        {
            var user = User.Create(
                Guid.NewGuid(),
                "Admin User",
                "admin@test.com",
                "hashed_password",
                true,
                DateTime.UtcNow
            );

            var query = new GetCurrentUserQuery();

            _baseUserServiceMock
                .Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Roles.Should().BeEquivalentTo(user.Roles);
        }
    }
}
