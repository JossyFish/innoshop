using MediatR;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.ResetPassword
{
    public sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        ICacheUsersRepository _cache;
        IConfirmCodeGenerator _confirmCodeGenerator;
        IUsersRepository _usersRepository;
        IEmailService _emailService;
        public ResetPasswordHandler(ICacheUsersRepository cache, IConfirmCodeGenerator confirmCodeGenerator, IUsersRepository usersRepository, 
                                    IEmailService emailService)
        {
            _cache = cache;
            _confirmCodeGenerator = confirmCodeGenerator;
            _usersRepository = usersRepository;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetByEmailAsync(command.Email, cancellationToken);

            if (user == null)
                throw new UserNotFoundException(command.Email);

            await _cache.RemoveResetPasswordDataAsync(user.Id, cancellationToken);

            var resetPasswordCode = _confirmCodeGenerator.Generate();

            var resetPasswordData = new ResetPasswordData(
                id: user.Id,
                email: user.Email,
                confirmationCode: resetPasswordCode
            );

            await _cache.SaveResetPasswordDataAsync(resetPasswordData, cancellationToken);

            await _emailService.SendResetPasswordAsync(user.Email, resetPasswordCode, cancellationToken);

            return Unit.Value;
        }
    }
}
