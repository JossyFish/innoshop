using FluentValidation;

namespace UserService.Application.Commands.ConfirmNewPassword
{
    public class ConfirmNewPasswordValidator : AbstractValidator<ConfirmNewPasswordCommand>
    {
        public ConfirmNewPasswordValidator() 
        {
            RuleFor(x => x.Email)
                 .NotEmpty().WithMessage("Email is required")
                 .EmailAddress().WithMessage("Wrong email format")
                 .MaximumLength(255).WithMessage("Email must not be higher than 255 characters");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be hier than 7 characters")
                .MaximumLength(100).WithMessage("Password must not be higher than 100 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~]").WithMessage("Password must contain at least one specific symbol");

            RuleFor(x => x.ConfirmationCode)
                .NotEmpty().WithMessage("ConfirmationCode is required")
                .Length(6).WithMessage("ConfirmationCode must be 6 characters")
                .Must(x => x.All(char.IsDigit)).WithMessage("ConfirmationCode must contain only digits");
        }
    }
}
