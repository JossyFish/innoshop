using Auth.Core.Services.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;
using ProductService.Application.Commands.BulkProduct;
using ProductService.Domain.Enums;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using Xunit;

namespace ProductService.UnitTests.Commands
{
    public class BulkProductTests
    {
        Mock<IProductRepository> _productRepositoryMock;
        Mock<IUserContext> _userContextMock;
        BulkProductHandler _handler;

        public BulkProductTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _userContextMock = new Mock<IUserContext>();
            _handler = new BulkProductHandler(_productRepositoryMock.Object, _userContextMock.Object);
        }

        [Theory]
        [InlineData(ProductOperations.Delete)]
        [InlineData(ProductOperations.Deactivate)]
        [InlineData(ProductOperations.Activate)]
        public async Task Handle_ValidCommandWithUserProducts_ShouldPerformOperation(ProductOperations operation)
        {
            var userId = Guid.NewGuid();
            var productIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var command = new BulkProductCommand
            {
                ProductIds = productIds,
                Operation = operation
            };

            var userProducts = new List<Product>
            {
                Product.Create(productIds[0], "Product1", "Desc1", new Price(100, Currency.USD), 10, true, userId, DateTime.UtcNow),
                Product.Create(productIds[1], "Product2", "Desc2", new Price(200, Currency.USD), 5, true, userId, DateTime.UtcNow)
            };

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _productRepositoryMock
                .Setup(x => x.GetByIdsAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProducts);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);

            switch (operation)
            {
                case ProductOperations.Delete:
                    _productRepositoryMock.Verify(x => x.BulkDeleteAsync(productIds, It.IsAny<CancellationToken>()), Times.Once);
                    break;
                case ProductOperations.Deactivate:
                    _productRepositoryMock.Verify(x => x.BulkUpdateActiveAsync(productIds, false, It.IsAny<CancellationToken>()), Times.Once);
                    break;
                case ProductOperations.Activate:
                    _productRepositoryMock.Verify(x => x.BulkUpdateActiveAsync(productIds, true, It.IsAny<CancellationToken>()), Times.Once);
                    break;
            }
        }

        [Fact]
        public async Task Handle_UserTriesToAccessOtherUserProducts_ShouldThrowProductAccessDeniedException()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var productIds = new[] { Guid.NewGuid() };
            var command = new BulkProductCommand
            {
                ProductIds = productIds,
                Operation = ProductOperations.Delete
            };

            var mixedProducts = new List<Product>
            {
                Product.Create(productIds[0], "Product1", "Desc1", new Price(100, Currency.USD), 10, true, otherUserId, DateTime.UtcNow)
            };

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _productRepositoryMock
                .Setup(x => x.GetByIdsAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mixedProducts);

            var exception = await Assert.ThrowsAsync<ProductAccessDeniedException>(() =>
                _handler.Handle(command, CancellationToken.None));

            exception.ProductId.Should().Be(productIds[0]);
            exception.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task Handle_InvalidOperation_ShouldThrowInvalidOperationException()
        {
            var userId = Guid.NewGuid();
            var productIds = new[] { Guid.NewGuid() };
            var command = new BulkProductCommand
            {
                ProductIds = productIds,
                Operation = (ProductOperations)999 
            };

            var userProducts = new List<Product>
            {
                Product.Create(productIds[0], "Product1", "Desc1", new Price(100, Currency.USD), 10, true, userId, DateTime.UtcNow)
            };

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _productRepositoryMock
                .Setup(x => x.GetByIdsAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProducts);

            await Assert.ThrowsAsync<InvalidOperation>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EmptyProductIds_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var command = new BulkProductCommand
            {
                ProductIds = Array.Empty<Guid>(),
                Operation = ProductOperations.Delete
            };

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _productRepositoryMock
                .Setup(x => x.GetByIdsAsync(Array.Empty<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            _productRepositoryMock.Verify(x => x.BulkDeleteAsync(Array.Empty<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

