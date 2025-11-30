using FluentValidation;

namespace UserService.Application.Commands.ConfirmRegisterCode
{
    public class ConfirmRegisterCodeValidator : AbstractValidator<ConfirmRegisterCodeCommand>
    {
        public ConfirmRegisterCodeValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Wrong email format")
                .MaximumLength(255).WithMessage("Email must not be higher than 255 characters");

            RuleFor(x => x.ConfirmationCode)
                .NotEmpty().WithMessage("ConfirmationCode is required")
                .Length(6).WithMessage("ConfirmationCode must be 6 characters")
                .Must(x => x.All(char.IsDigit)).WithMessage("ConfirmationCode must contain only digits");
        }
    }
}
