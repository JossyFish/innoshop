using MediatR;
using Microsoft.Extensions.Options;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.ChangeEmail
{
    public sealed class ChangeEmailHandler : IRequestHandler<ChangeEmailCommand, Unit>
    {
        IBaseUserService _baseUserHandler;
        ICacheUsersRepository _cache;
        IConfirmCodeGenerator _confirmCodeGenerator;
        IUsersRepository _usersRepository;
        IEmailService _emailService;
        ILinkTokenGenerator _linkTokenGenerator;
       AppSettings _appSettings;

        public ChangeEmailHandler(IBaseUserService baseUserHandler, ICacheUsersRepository cache,IConfirmCodeGenerator confirmCodeGenerator,
            IUsersRepository usersRepository, IEmailService emailService, ILinkTokenGenerator linkTokenGenerator, IOptions<AppSettings> appSettings)
        {
            _baseUserHandler = baseUserHandler;
            _cache = cache;
            _confirmCodeGenerator = confirmCodeGenerator;
            _usersRepository = usersRepository;
            _emailService = emailService;
            _linkTokenGenerator = linkTokenGenerator;
            _appSettings = appSettings.Value;
        }

        public async Task<Unit> Handle(ChangeEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _baseUserHandler.GetCurrentUserAsync(cancellationToken);

            await _cache.RemoveChangeEmailDataAsync(user.Id, cancellationToken);

           
            var existingUser = await _usersRepository.GetByEmailAsync(command.NewEmail, cancellationToken);
            if (existingUser != null) 
                throw new UserAlreadyExistException(existingUser.Email);

            var confirmEmailCode = _confirmCodeGenerator.Generate();
            var linkToken = _linkTokenGenerator.GenerateToken(user.Email);
            //var confirmationLink = $"{_appSettings.BaseUrl}/api/User/confirm-newEmail-link?token={Uri.EscapeDataString(linkToken)}";
            var confirmationLink = $"{_appSettings.FrontendUrl}/confirm-email-change?token={Uri.EscapeDataString(linkToken)}";

            var changeEmailData = new ChangeEmailData(
              id: user.Id,
              oldEmail: user.Email,
              newEmail: command.NewEmail,
              confirmationCode: confirmEmailCode,
              linkToken: linkToken
            );

            await _cache.SaveChangeEmailDataAsync(changeEmailData, cancellationToken);

            await _emailService.SendConfirmationEmailAsync(command.NewEmail, confirmEmailCode, confirmationLink, cancellationToken);

            return Unit.Value;
        }
    }
}
