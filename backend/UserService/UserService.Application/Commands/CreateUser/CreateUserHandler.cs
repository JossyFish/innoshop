using MediatR;
using Microsoft.Extensions.Options;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.CreateUser
{
    public sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, Unit>
    {
        ICacheUsersRepository _cache;
        IPasswordHasher _passwordHasher;
        IConfirmCodeGenerator _confirmCodeGenerator;
        IUsersRepository _usersRepository;
        IEmailService _emailService;
        ILinkTokenGenerator _linkTokenGenerator;
        private readonly AppSettings _appSettings;
        public CreateUserHandler(ICacheUsersRepository cache, IPasswordHasher passwordHasher, IConfirmCodeGenerator confirmCodeGenerator, 
                                 IUsersRepository usersRepository, IEmailService emailService, ILinkTokenGenerator linkTokenGenerator,
                                 IOptions<AppSettings> appSettings) 
        {
            _cache = cache;
            _passwordHasher = passwordHasher;
            _confirmCodeGenerator = confirmCodeGenerator;
            _usersRepository = usersRepository;
            _emailService = emailService;
            _linkTokenGenerator = linkTokenGenerator;
            _appSettings = appSettings.Value;
        }

        public async Task<Unit> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await _usersRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (existingUser != null)
                throw new UserAlreadyExistException(command.Email);

            await _cache.RemoveRegistrationDataByEmailAsync(command.Email, cancellationToken);   

            var hashPassword = _passwordHasher.Generate(command.Password);
            var confirmUserCode = _confirmCodeGenerator.Generate();
            var linkToken = _linkTokenGenerator.GenerateToken(command.Email);

            //var confirmationLink = $"{_appSettings.BaseUrl}/api/User/confirm-registration-link?token={Uri.EscapeDataString(linkToken)}";
            var confirmationLink = $"{_appSettings.FrontendUrl}/confirm-register?token={Uri.EscapeDataString(linkToken)}";

            var creationUserData = new CreationUserData(
                id: Guid.NewGuid(),
                name: command.Name,
                email: command.Email,
                passwordHash: hashPassword,
                confirmationCode: confirmUserCode,
                linkToken: linkToken
            );

            await _cache.SaveRegistrationDataAsync(creationUserData, cancellationToken);

            await _emailService.SendConfirmationEmailAsync(command.Email, confirmUserCode, confirmationLink, cancellationToken);

            return Unit.Value;
        }
    }
}
