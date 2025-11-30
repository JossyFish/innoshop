using FluentValidation;

namespace UserService.Application.Commands.ChangePassword
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordValidator() 
        {
            RuleFor(x => x.CurrentPassword)
               .NotEmpty().WithMessage("Password is required");

            RuleFor(x => x.NewPassword)
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
