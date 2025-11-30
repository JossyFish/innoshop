using FluentValidation;

namespace ProductService.Application.Commands.ChangeProduct
{
    public class ChangeProductValidator : AbstractValidator<ChangeProductCommand>
    {
        public ChangeProductValidator()
        {
            RuleFor(x => x.ProductId)
               .NotEmpty().WithMessage("Product ID is required");

            RuleFor(x => x.Name)
               .MinimumLength(2).WithMessage("Name must be higher than 1 character")
               .MaximumLength(100).WithMessage("Name must not be higher than 100 characters")
               .When(x => !string.IsNullOrEmpty(x.Name)); ;

            RuleFor(x => x.Description)
               .MaximumLength(1000).WithMessage("Description must not be higher than 100 characters");

            RuleFor(x => x.Cost)
                .GreaterThanOrEqualTo(0).WithMessage("Cost must be greater than or equal to 0")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Currency)
                  .IsInEnum().WithMessage("Invalid currency type")
                  .When(x => x.Cost.HasValue);

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity must be greater than or equal to 0")
                .When(x => x.Quantity.HasValue);
        }
    }
}
