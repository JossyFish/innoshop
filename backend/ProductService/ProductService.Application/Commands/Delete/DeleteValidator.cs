using FluentValidation;

namespace ProductService.Application.Commands.Delete
{
    public class DeleteValidator : AbstractValidator<DeleteCommand>
    {
        public DeleteValidator() 
        {
            RuleFor(x => x.ProductId)
             .NotEmpty().WithMessage("Product ID is required");
        }
    }
}
