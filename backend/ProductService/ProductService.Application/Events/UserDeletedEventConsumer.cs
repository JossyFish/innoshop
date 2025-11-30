using MassTransit;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Interfaces;
using UserService.Application.Events;


namespace ProductService.Application.Events
{
    public class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        IProductRepository _productRepository;
        ILogger<UserDeletedEventConsumer> _logger;

        public UserDeletedEventConsumer (IProductRepository productRepository, ILogger<UserDeletedEventConsumer> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task Consume (ConsumeContext<UserDeletedEvent> context)
        {
            var message = context.Message;

            try
            {
                await _productRepository.DeleteAllAsync(message.UserId, context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete products for user {UserId}", message.UserId);

                throw;
            }
        }
     }
}
