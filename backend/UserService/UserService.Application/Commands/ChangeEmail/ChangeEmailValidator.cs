using FluentValidation;

namespace UserService.Application.Commands.ChangeEmail
{
    public class ChangeEmailValidator : AbstractValidator<ChangeEmailCommand>
    {
        public ChangeEmailValidator()
        {
            RuleFor(x => x.NewEmail)
                    .NotEmpty().WithMessage("Email is required")
                    .EmailAddress().WithMessage("Wrong email format")
                    .MaximumLength(255).WithMessage("Email must not be higher than 255 characters");
        }
    }
}
