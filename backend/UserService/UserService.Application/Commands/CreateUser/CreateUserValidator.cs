using FluentValidation;

namespace UserService.Application.Commands.CreateUser
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be higher than 1 character")
                .MaximumLength(100).WithMessage("Name must not be higher than 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Wrong email format")
                .MaximumLength(255).WithMessage("Email must not be higher than 255 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be hier than 7 characters")
                .MaximumLength(100).WithMessage("Password must not be higher than 100 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~]").WithMessage("Password must contain at least one specific symbol");
        }
    }
}
