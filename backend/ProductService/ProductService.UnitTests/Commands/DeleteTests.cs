using Auth.Core.Services.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;
using ProductService.Application.Commands.Delete;
using ProductService.Application.Interfaces;
using ProductService.Domain.Enums;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using Xunit;

namespace ProductService.UnitTests.Commands
{
    public class DeleteTests
    {
        Mock<IProductRepository> _productRepositoryMock;
        Mock<IUserContext> _userContextMock;
        Mock<IFindProductService> _findProductServiceMock;
        DeleteHandler _handler;

        public DeleteTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _userContextMock = new Mock<IUserContext>();
            _findProductServiceMock = new Mock<IFindProductService>();
            _handler = new DeleteHandler(
                _productRepositoryMock.Object,
                _userContextMock.Object,
                _findProductServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidUserProduct_ShouldDeleteProduct()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new DeleteCommand { ProductId = productId };

            var product = Product.Create(
                productId, "Product", "Description",
                new Price(100, Currency.USD), 10, true, userId, DateTime.UtcNow);

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _findProductServiceMock
                .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _productRepositoryMock
                .Setup(x => x.DeleteByIdAsync(productId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            _productRepositoryMock.Verify(x => x.DeleteByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserTriesToDeleteOtherUserProduct_ShouldThrowProductAccessDeniedException()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new DeleteCommand { ProductId = productId };

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
            _productRepositoryMock.Verify(x => x.DeleteByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new DeleteCommand { ProductId = productId };

            var product = Product.Create(
                productId, "Product", "Description",
                new Price(100, Currency.USD), 10, true, userId, DateTime.UtcNow);

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _findProductServiceMock
                .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _productRepositoryMock
                .Setup(x => x.DeleteByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Delete failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
