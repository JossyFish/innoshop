using FluentValidation;

namespace UserService.Application.Commands.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator() 
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required")
               .EmailAddress().WithMessage("Wrong email format")
               .MaximumLength(255).WithMessage("Email must not be higher than 255 characters");
        }
    }
}
