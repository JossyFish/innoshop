using FluentValidation;

namespace UserService.Application.Commands.ChangeProfile
{
    public class ChangeProfileValidator : AbstractValidator<ChangeProfileCommand>
    {
        public ChangeProfileValidator() 
        {
            RuleFor(x => x.NewName)
              .NotEmpty().WithMessage("Name is required")
              .MinimumLength(2).WithMessage("Name must be higher than 1 character")
              .MaximumLength(100).WithMessage("Name must not be higher than 100 characters");
        }
    }
}
