using FluentAssertions;
using Moq;
using ProductService.Application.Queries.GetProducts;
using ProductService.Domain.Enums;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using Xunit;

namespace ProductService.UnitTests.Queries
{
    public class GetProductsTests
    {
        Mock<IProductRepository> _productRepositoryMock;
        GetProductsHandler _handler;

        public GetProductsTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new GetProductsHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidQueryWithFilters_ShouldReturnFilteredProducts()
        {
            var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 20,
                Name = "Test",
                MinPrice = 50,
                MaxPrice = 200,
                Currency = Currency.USD,
                MinQuantity = 1,
                UserIds = userIds, 
                MinCreatedAt = DateTime.UtcNow.AddDays(-7),
                MaxCreatedAt = DateTime.UtcNow
            };

            var products = new List<Product>
            {
                Product.Create(Guid.NewGuid(), "Test Product", "Desc", new Price(100, Currency.USD), 5, true, userIds[0], DateTime.UtcNow)
            };

            _productRepositoryMock
                .Setup(x => x.GetFilteredProductsAsync(
                    It.IsAny<Filters>(),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, 1));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Products.Should().HaveCount(1);
            result.TotalCount.Should().Be(1);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(20);
            result.TotalPages.Should().Be(1);

            _productRepositoryMock.Verify(x => x.GetFilteredProductsAsync(
                It.Is<Filters>(f =>
                    f.Name == "Test" &&
                    f.MinPrice == 50 &&
                    f.MaxPrice == 200 &&
                    f.Currency == Currency.USD &&
                    f.MinQuantity == 1 &&
                    f.UserIds != null &&
                    f.UserIds.Count == 2 &&
                    f.UserIds[0] == userIds[0] &&
                    f.UserIds[1] == userIds[1] && 
                    f.MinCreatedAt.HasValue &&
                    f.MaxCreatedAt.HasValue),
                It.Is<Pagination>(p => p.PageNumber == 1 && p.PageSize == 20),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_FilterByMultipleUserIds_ShouldReturnProductsFromSpecifiedUsers()
        {
            var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 20,
                UserIds = userIds 
            };

            var products = new List<Product>
            {
                Product.Create(Guid.NewGuid(), "Product 1", "Desc", new Price(100, Currency.USD), 5, true, userIds[0], DateTime.UtcNow),
                Product.Create(Guid.NewGuid(), "Product 2", "Desc", new Price(200, Currency.USD), 3, true, userIds[1], DateTime.UtcNow)
            };

            _productRepositoryMock
                .Setup(x => x.GetFilteredProductsAsync(
                    It.Is<Filters>(f => f.UserIds != null && f.UserIds.Count == 3),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, 2));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Products.Should().HaveCount(2);
            result.Products[0].UserId.Should().Be(userIds[0]);
            result.Products[1].UserId.Should().Be(userIds[1]);
        }

        [Fact]
        public async Task Handle_FilterBySingleUserId_ShouldWorkCorrectly()
        {
            var singleUserId = new List<Guid> { Guid.NewGuid() }; 
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 20,
                UserIds = singleUserId
            };

            var products = new List<Product>
            {
                Product.Create(Guid.NewGuid(), "Product 1", "Desc", new Price(100, Currency.USD), 5, true, singleUserId[0], DateTime.UtcNow)
            };

            _productRepositoryMock
                .Setup(x => x.GetFilteredProductsAsync(
                    It.Is<Filters>(f => f.UserIds != null && f.UserIds.Count == 1),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, 1));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Products.Should().HaveCount(1);
            result.Products[0].UserId.Should().Be(singleUserId[0]);
        }

        [Fact]
        public async Task Handle_EmptyUserIdsList_ShouldReturnAllUsersProducts()
        {
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 20,
                UserIds = new List<Guid>()
            };

            var products = new List<Product>
            {
                Product.Create(Guid.NewGuid(), "Product 1", "Desc", new Price(100, Currency.USD), 5, true, Guid.NewGuid(), DateTime.UtcNow),
                Product.Create(Guid.NewGuid(), "Product 2", "Desc", new Price(200, Currency.USD), 3, true, Guid.NewGuid(), DateTime.UtcNow)
            };

            _productRepositoryMock
                .Setup(x => x.GetFilteredProductsAsync(
                    It.Is<Filters>(f => f.UserIds != null && f.UserIds.Count == 0),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, 2));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Products.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_NullUserIds_ShouldNotFilterByUsers()
        {
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 20,
                UserIds = null 
            };

            var products = new List<Product>
            {
                Product.Create(Guid.NewGuid(), "Product 1", "Desc", new Price(100, Currency.USD), 5, true, Guid.NewGuid(), DateTime.UtcNow)
            };

            _productRepositoryMock
                .Setup(x => x.GetFilteredProductsAsync(
                    It.Is<Filters>(f => f.UserIds == null),
                    It.IsAny<Pagination>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, 1));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Products.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(0, 10, 1, 10)]
        [InlineData(-1, 500, 1, 100)]
        [InlineData(2, 25, 2, 25)]
        public async Task Handle_PaginationValues_ShouldBeClampedCorrectly(int inputPage, int inputSize, int expectedPage, int expectedSize)
        {
            var query = new GetProductsQuery { PageNumber = inputPage, PageSize = inputSize };

            _productRepositoryMock
                .Setup(x => x.GetFilteredProductsAsync(It.IsAny<Filters>(), It.IsAny<Pagination>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Product>(), 0));

            var result = await _handler.Handle(query, CancellationToken.None);

            _productRepositoryMock.Verify(x => x.GetFilteredProductsAsync(
                It.IsAny<Filters>(),
                It.Is<Pagination>(p => p.PageNumber == expectedPage && p.PageSize == expectedSize),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyFilters_ShouldPassNullValues()
        {
            var query = new GetProductsQuery { PageNumber = 1, PageSize = 10 };

            _productRepositoryMock
                .Setup(x => x.GetFilteredProductsAsync(It.IsAny<Filters>(), It.IsAny<Pagination>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Product>(), 0));

            var result = await _handler.Handle(query, CancellationToken.None);

            _productRepositoryMock.Verify(x => x.GetFilteredProductsAsync(
                It.Is<Filters>(f =>
                    f.Name == null &&
                    f.MinPrice == null &&
                    f.MaxPrice == null &&
                    f.Currency == null &&
                    f.MinQuantity == null &&
                    f.UserIds != null && 
                    f.UserIds.Count == 0 &&
                    f.MinCreatedAt == null &&
                    f.MaxCreatedAt == null),
                It.IsAny<Pagination>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_MultiplePages_ShouldCalculateTotalPagesCorrectly()
        {
            var query = new GetProductsQuery { PageNumber = 2, PageSize = 10 };
            var totalCount = 25; 

            _productRepositoryMock
                .Setup(x => x.GetFilteredProductsAsync(It.IsAny<Filters>(), It.IsAny<Pagination>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Product>(), totalCount));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.TotalPages.Should().Be(3);
            result.TotalCount.Should().Be(25);
            result.PageNumber.Should().Be(2);
            result.PageSize.Should().Be(10);
        }
    }
}