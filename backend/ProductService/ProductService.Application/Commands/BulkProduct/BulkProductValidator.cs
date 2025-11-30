using FluentValidation;

namespace ProductService.Application.Commands.BulkProduct
{
    public class BulkProductValidator : AbstractValidator <BulkProductCommand>
    {
        public BulkProductValidator() 
        {
            RuleFor(x => x.ProductIds)
              .NotEmpty().WithMessage("Product ID is required");

            RuleFor(x => x.Operation)
            .IsInEnum().WithMessage("Invalid operation type");
        }
    }
}
