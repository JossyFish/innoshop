using FluentValidation;

namespace ProductService.Application.Commands.AddProduct
{
    public class AddProductValidator : AbstractValidator<AddProductCommand>
    {
        public AddProductValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Name is required")
               .MinimumLength(2).WithMessage("Name must be higher than 1 character")
               .MaximumLength(100).WithMessage("Name must not be higher than 100 characters");

            RuleFor(x => x.Description)
               .MaximumLength(1000).WithMessage("Description must not be higher than 100 characters");

            RuleFor(x => x.Cost)
                .NotEmpty().WithMessage("Cost is required")
                .GreaterThanOrEqualTo(0).WithMessage("Cost must be greater than or equal to 0");

            RuleFor(x => x.Currency)
                  .IsInEnum().WithMessage("Invalid currency type");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity must be greater than or equal to 0");
        }
    }
}
