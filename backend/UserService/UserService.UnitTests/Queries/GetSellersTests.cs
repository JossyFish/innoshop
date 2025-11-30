using FluentAssertions;
using Moq;
using UserService.Application.Queries.GetSellers;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.UnitTests.Queries
{
    public class GetSellersTests
    {
        Mock<IUsersRepository> _usersRepositoryMock;
        GetSellersHandler _handler;

        public GetSellersTests()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _handler = new GetSellersHandler(_usersRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidQuery_ShouldReturnPaginatedSellers()
        {
            var query = new GetSellersQuery
            {
                PageNumber = 1,
                PageSize = 20,
                Search = "john"
            };

            var users = new List<User>
            {
                User.Create(Guid.NewGuid(), "John Doe", "john@test.com", "hash1", true, DateTime.UtcNow),
                User.Create(Guid.NewGuid(), "John Smith", "john.smith@test.com", "hash2", true, DateTime.UtcNow)
            };

            var totalCount = 15;

            _usersRepositoryMock
                .Setup(x => x.GetFilterUsersAsync(
                    It.IsAny<UserFilters>(),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((users, totalCount));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Sellers.Should().HaveCount(2);
            result.TotalCount.Should().Be(15);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(20);
            result.TotalPages.Should().Be(1); 

            _usersRepositoryMock.Verify(x => x.GetFilterUsersAsync(
                It.Is<UserFilters>(f =>
                    f.Search == "john" &&
                    f.IsActive == true &&
                    f.RoleName == "User"),
                It.Is<Pagination>(p => p.PageNumber == 1 && p.PageSize == 20),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(0, 10, 1, 10)]   
        [InlineData(-1, 500, 1, 100)] 
        [InlineData(2, 25, 2, 25)]   
        [InlineData(1, 0, 1, 1)]
        public async Task Handle_PaginationValues_ShouldBeClampedCorrectly(
            int inputPage, int inputSize, int expectedPage, int expectedSize)
        {
            var query = new GetSellersQuery
            {
                PageNumber = inputPage,
                PageSize = inputSize
            };

            _usersRepositoryMock
                .Setup(x => x.GetFilterUsersAsync(
                    It.IsAny<UserFilters>(),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<User>(), 0));

            var result = await _handler.Handle(query, CancellationToken.None);

            _usersRepositoryMock.Verify(x => x.GetFilterUsersAsync(
                It.IsAny<UserFilters>(),
                It.Is<Pagination>(p => p.PageNumber == expectedPage && p.PageSize == expectedSize),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoSearchFilter_ShouldPassNullSearch()
        {
            var query = new GetSellersQuery { PageNumber = 1, PageSize = 10 };
          
            _usersRepositoryMock
                .Setup(x => x.GetFilterUsersAsync(
                    It.IsAny<UserFilters>(),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<User>(), 0));

            var result = await _handler.Handle(query, CancellationToken.None);

            _usersRepositoryMock.Verify(x => x.GetFilterUsersAsync(
                It.Is<UserFilters>(f => f.Search == null),
                It.IsAny<Pagination>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyResult_ShouldReturnEmptySellersList()
        {
            var query = new GetSellersQuery { PageNumber = 1, PageSize = 20 };
            var emptyUsers = new List<User>();
            var totalCount = 0;

            _usersRepositoryMock
                .Setup(x => x.GetFilterUsersAsync(
                    It.IsAny<UserFilters>(),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((emptyUsers, totalCount));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Sellers.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
            result.TotalPages.Should().Be(0);
        }

        [Fact]
        public async Task Handle_MultiplePages_ShouldCalculateTotalPagesCorrectly()
        {
            var query = new GetSellersQuery { PageNumber = 3, PageSize = 10 };
            var totalCount = 25;

            _usersRepositoryMock
                .Setup(x => x.GetFilterUsersAsync(
                    It.IsAny<UserFilters>(),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<User>(), totalCount));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.TotalPages.Should().Be(3);
            result.TotalCount.Should().Be(25);
            result.PageNumber.Should().Be(3);
            result.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task Handle_UserData_ShouldMapToSellerResponseCorrectly()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var query = new GetSellersQuery { PageNumber = 1, PageSize = 10 };

            var users = new List<User>
            {
                User.Create(userId1, "Seller One", "seller1@test.com", "hash1", true, DateTime.UtcNow),
                User.Create(userId2, "Seller Two", "seller2@test.com", "hash2", true, DateTime.UtcNow)
            };

            _usersRepositoryMock
                .Setup(x => x.GetFilterUsersAsync(
                    It.IsAny<UserFilters>(),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((users, 2));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Sellers.Should().HaveCount(2);

            result.Sellers[0].Id.Should().Be(userId1);
            result.Sellers[0].Name.Should().Be("Seller One");

            result.Sellers[1].Id.Should().Be(userId2);
            result.Sellers[1].Name.Should().Be("Seller Two");
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var query = new GetSellersQuery { PageNumber = 1, PageSize = 10 };

            _usersRepositoryMock
                .Setup(x => x.GetFilterUsersAsync(
                    It.IsAny<UserFilters>(),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ExactPageSize_ShouldCalculatePagesCorrectly()
        {
            var query = new GetSellersQuery { PageNumber = 2, PageSize = 10 };
            var totalCount = 20;

            _usersRepositoryMock
                .Setup(x => x.GetFilterUsersAsync(
                    It.IsAny<UserFilters>(),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<User>(), totalCount));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.TotalPages.Should().Be(2);
            result.TotalCount.Should().Be(20);
        }
    }
}
