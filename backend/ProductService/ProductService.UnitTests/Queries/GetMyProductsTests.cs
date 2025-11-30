using Auth.Core.Services.Interfaces;
using FluentAssertions;
using Moq;
using ProductService.Application.Queries.GetMyProducts;
using ProductService.Domain.Enums;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using Xunit;

namespace ProductService.UnitTests.Queries
{
    public class GetMyProductsTests
    {
        Mock<IProductRepository> _productRepositoryMock;
        Mock<IUserContext> _userContextMock;
        GetMyProductsHandler _handler;

        public GetMyProductsTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _userContextMock = new Mock<IUserContext>();
            _handler = new GetMyProductsHandler(_productRepositoryMock.Object, _userContextMock.Object);
        }

        [Fact]
        public async Task Handle_ValidQuery_ShouldReturnPaginatedProducts()
        {
            var userId = Guid.NewGuid();
            var query = new GetMyProductsQuery { PageNumber = 1, PageSize = 10 };

            var products = new List<Product>
            {
                Product.Create(Guid.NewGuid(), "Product 1", "Desc 1", new Price(100, Currency.USD), 5, true, userId, DateTime.UtcNow),
                Product.Create(Guid.NewGuid(), "Product 2", "Desc 2", new Price(200, Currency.EUR), 3, true, userId, DateTime.UtcNow)
            };

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _productRepositoryMock
                .Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, 2));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Products.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(1);

            result.Products[0].Name.Should().Be("Product 1");
            result.Products[1].Name.Should().Be("Product 2");
        }

        [Theory]
        [InlineData(0, 10, 1, 10)]  
        [InlineData(1, 0, 1, 1)]    
        [InlineData(1, 200, 1, 100)] 
        public async Task Handle_InvalidPagination_ShouldClampValues(int inputPage, int inputSize, int expectedPage, int expectedSize)
        {
            var userId = Guid.NewGuid();
            var query = new GetMyProductsQuery { PageNumber = inputPage, PageSize = inputSize };

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _productRepositoryMock
                .Setup(x => x.GetByUserIdAsync(userId, expectedPage, expectedSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Product>(), 0));

            var result = await _handler.Handle(query, CancellationToken.None);

            _productRepositoryMock.Verify(x => x.GetByUserIdAsync(
                userId, expectedPage, expectedSize, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoProducts_ShouldReturnEmptyList()
        {
            var userId = Guid.NewGuid();
            var query = new GetMyProductsQuery { PageNumber = 1, PageSize = 10 };

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _productRepositoryMock
                .Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Product>(), 0));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Products.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
            result.TotalPages.Should().Be(0);
        }
    }
}
