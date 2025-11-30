using FluentValidation;

namespace UserService.Application.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Wrong email format")
                .MaximumLength(255).WithMessage("Email must not be higher than 255 characters");

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage("Password is required");
        }
    }
}
