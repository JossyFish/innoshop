using FluentValidation;

namespace UserService.Application.Commands.DeleteById
{
    public class DeleteByIdValidator : AbstractValidator<DeleteByIdCommand>
    {
        public DeleteByIdValidator() 
        {
            RuleFor(x => x.Id)
              .NotEmpty().WithMessage("Id is required");
        }
    }
}
