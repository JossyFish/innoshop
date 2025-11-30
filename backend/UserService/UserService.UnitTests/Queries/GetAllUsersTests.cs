using FluentAssertions;
using Moq;
using UserService.Application.Queries.GetAllUsers;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Queries
{
    public class GetAllUsersTests
    {
        Mock<IUsersRepository> _usersRepositoryMock;
        GetAllUsersHandler _handler;

        public GetAllUsersTests()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _handler = new GetAllUsersHandler(_usersRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_UsersExist_ShouldReturnAllUsers()
        {
            var users = new List<User>
            {
                User.Create(Guid.NewGuid(), "User 1", "user1@test.com", "hash1", true, DateTime.UtcNow.AddDays(-10)),
                User.Create(Guid.NewGuid(), "User 2", "user2@test.com", "hash2", true, DateTime.UtcNow.AddDays(-5)),
                User.Create(Guid.NewGuid(), "User 3", "user3@test.com", "hash3", false, DateTime.UtcNow.AddDays(-1))
            };

            var query = new GetAllUsersQuery();

            _usersRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            result[0].Name.Should().Be("User 1");
            result[0].Email.Should().Be("user1@test.com");
            result[0].IsActive.Should().BeTrue();

            result[1].Name.Should().Be("User 2");
            result[1].Email.Should().Be("user2@test.com");
            result[1].IsActive.Should().BeTrue();

            result[2].Name.Should().Be("User 3");
            result[2].Email.Should().Be("user3@test.com");
            result[2].IsActive.Should().BeFalse();

            _usersRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoUsers_ShouldReturnEmptyList()
        {
            var users = new List<User>();
            var query = new GetAllUsersQuery();

            _usersRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _usersRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_MixedActiveUsers_ShouldReturnCorrectData()
        {
            var users = new List<User>
            {
                User.Create(Guid.NewGuid(), "Active User", "active@test.com", "hash1", true, DateTime.UtcNow),
                User.Create(Guid.NewGuid(), "Inactive User", "inactive@test.com", "hash2", false, DateTime.UtcNow)
            };

            var query = new GetAllUsersQuery();

            _usersRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].IsActive.Should().BeTrue();
            result[1].IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var query = new GetAllUsersQuery();

            _usersRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }
    }
}
