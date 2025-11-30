using Auth.Core.Services.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;
using ProductService.Application.Commands.AddProduct;
using ProductService.Domain.Enums;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using Xunit;

namespace ProductService.UnitTests.Commands
{
    public class AddProductTests
    {
        Mock<IUserContext> _userContextMock;
        Mock<IProductRepository> _productRepositoryMock;
        AddProductHandler _handler;

        public AddProductTests()
        {
            _userContextMock = new Mock<IUserContext>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new AddProductHandler(_userContextMock.Object, _productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateProductAndAddToRepository()
        {
            var userId = Guid.NewGuid();
            var command = new AddProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                Cost = 100.50m,
                Currency = Currency.USD,
                Quantity = 10,
                IsActive = true
            };

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _productRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);

            _userContextMock.Verify(x => x.GetCurrentUserId(), Times.Once);
            _productRepositoryMock.Verify(x => x.AddAsync(
                It.Is<Product>(p =>
                    p.Name == command.Name &&
                    p.Description == command.Description &&
                    p.Price.Cost == command.Cost &&
                    p.Price.Currency == command.Currency &&
                    p.Quantity == command.Quantity &&
                    p.IsActive == command.IsActive &&
                    p.UserId == userId &&
                    p.CreatedAt <= DateTime.UtcNow
                ),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var userId = Guid.NewGuid();
            var command = new AddProductCommand
            {
                Name = "Test Product",
                Cost = 100.50m,
                Currency = Currency.USD,
                Quantity = 10
            };

            _userContextMock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            _productRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
