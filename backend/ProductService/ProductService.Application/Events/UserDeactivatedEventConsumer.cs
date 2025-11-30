using MassTransit;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Interfaces;
using UserService.Application.Events;

namespace ProductService.Application.Events
{
    public class UserDeactivatedEventConsumer : IConsumer<UserDeactivatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<UserDeactivatedEventConsumer> _logger;

        public UserDeactivatedEventConsumer(
            IProductRepository productRepository,
            ILogger<UserDeactivatedEventConsumer> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task Consume (ConsumeContext<UserDeactivatedEvent> context)
        {
            var message = context.Message;
            _logger.LogError("Deactivate products {message}", message.UserId);

            try
            {
                await _productRepository.DeactivateAllAsync(message.UserId, context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deactivate products for user {UserId}", message.UserId);

                throw;
            }

        }
    }
}
