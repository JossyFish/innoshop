using FluentAssertions;
using Moq;
using ProductService.Application.Interfaces;
using ProductService.Application.Queries.GetById;
using ProductService.Domain.Enums;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using Xunit;

namespace ProductService.UnitTests.Queries
{
    public class GetByIdTests
    {
        Mock<IFindProductService> _findProductServiceMock;
        GetByIdHandler _handler;

        public GetByIdTests()
        {
            _findProductServiceMock = new Mock<IFindProductService>();
            _handler = new GetByIdHandler(_findProductServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ProductExists_ShouldReturnProduct()
        {
            var productId = Guid.NewGuid();
            var query = new GetByIdQuery { Id = productId };

            var product = Product.Create(
                productId,
                "Test Product",
                "Test Description",
                new Price(100.50m, Currency.USD),
                10,
                true,
                Guid.NewGuid(),
                DateTime.UtcNow
            );

            _findProductServiceMock
                .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Test Product");
            result.Description.Should().Be("Test Description");
            result.Price.Cost.Should().Be(100.50m);
            result.Quantity.Should().Be(10);
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldThrowProductNotFoundException()
        {
            var productId = Guid.NewGuid();
            var query = new GetByIdQuery { Id = productId };

            _findProductServiceMock
                .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            await Assert.ThrowsAsync<ProductNotFoundException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }
    }
}
