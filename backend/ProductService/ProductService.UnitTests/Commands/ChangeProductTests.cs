using Auth.Core.Services.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;
using ProductService.Application.Commands.ChangeProduct;
using ProductService.Application.Interfaces;
using ProductService.Domain.Enums;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using Xunit;

namespace ProductService.UnitTests.Commands
{
    public class ChangeProductTests
    {
        Mock<IProductRepository> _productRepositoryMock;
        Mock<IUserContext> _userContextMock;
        Mock<IFindProductService> _findProductServiceMock;
        ChangeProductHandler _handler;

        public ChangeProductTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _userContextMock = new Mock<IUserContext>();
            _findProductServiceMock = new Mock<IFindProductService>();
            _handler = new ChangeProductHandler(
                _productRepositoryMock.Object,
                _userContextMock.Object,
                _findProductServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidChanges_ShouldUpdateProduct()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new ChangeProductCommand
            {
                ProductId = productId,
                Name = "Updated Name",
                Description = "Updated Description",
                Cost = 150.75m,
                Currency = Currency.EUR,
                Quantity = 20,
                IsActive = false
            };

            var existingProduct = Product.Create(
                productId, "Old Name", "Old Description",
                new Price(100, Currency.USD), 10, true, userId, DateTime.UtcNow);

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _findProductServiceMock
                .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);
            _productRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);

            existingProduct.Name.Should().Be("Updated Name");
            existingProduct.Description.Should().Be("Updated Description");
            existingProduct.Price.Cost.Should().Be(150.75m);
            existingProduct.Price.Currency.Should().Be(Currency.EUR);
            existingProduct.Quantity.Should().Be(20);
            existingProduct.IsActive.Should().BeFalse();

            _productRepositoryMock.Verify(x => x.UpdateAsync(existingProduct, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PartialChanges_ShouldUpdateOnlyProvidedFields()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new ChangeProductCommand
            {
                ProductId = productId,
                Name = "Only Name Updated",
            };

            var existingProduct = Product.Create(
                productId, "Old Name", "Old Description",
                new Price(100, Currency.USD), 10, true, userId, DateTime.UtcNow);

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _findProductServiceMock
                .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);
            _productRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);

            existingProduct.Name.Should().Be("Only Name Updated");
            existingProduct.Description.Should().Be("Old Description"); 
            existingProduct.Price.Cost.Should().Be(100); 
            existingProduct.Quantity.Should().Be(10);
            existingProduct.IsActive.Should().BeTrue(); 
        }

        [Fact]
        public async Task Handle_UserTriesToChangeOtherUserProduct_ShouldThrowProductAccessDeniedException()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new ChangeProductCommand { ProductId = productId };

            var otherUserProduct = Product.Create(
                productId, "Product", "Description",
                new Price(100, Currency.USD), 10, true, otherUserId, DateTime.UtcNow);

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _findProductServiceMock
                .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherUserProduct);

            var exception = await Assert.ThrowsAsync<ProductAccessDeniedException>(() =>
                _handler.Handle(command, CancellationToken.None));

            exception.ProductId.Should().Be(productId);
            exception.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task Handle_ChangeOnlyPrice_ShouldUpdatePriceCorrectly()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new ChangeProductCommand
            {
                ProductId = productId,
                Cost = 200m,
                Currency = Currency.BYN
            };

            var existingProduct = Product.Create(
                productId, "Product", "Description",
                new Price(100, Currency.USD), 10, true, userId, DateTime.UtcNow);

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _findProductServiceMock
                .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);
            _productRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            existingProduct.Price.Cost.Should().Be(200m);
            existingProduct.Price.Currency.Should().Be(Currency.BYN);
        }
    }
}
