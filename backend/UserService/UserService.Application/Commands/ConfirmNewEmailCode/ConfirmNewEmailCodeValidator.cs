using FluentValidation;

namespace UserService.Application.Commands.ConfirmNewEmailCode
{
    public class ConfirmNewEmailCodeValidator : AbstractValidator<ConfirmNewEmailCodeCommand>
    {
        public ConfirmNewEmailCodeValidator()
        {
            RuleFor(x => x.ConfirmationCode)
           .NotEmpty().WithMessage("ConfirmationCode is required")
           .Length(6).WithMessage("ConfirmationCode must be 6 characters")
           .Must(x => x.All(char.IsDigit)).WithMessage("ConfirmationCode must contain only digits");
        }
    }
}
