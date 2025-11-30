using UserService.Domain.Models;

namespace UserService.Domain.Interfaces
{
    public interface ICacheUsersRepository
    {
        Task SaveRegistrationDataAsync(CreationUserData creationUserData, CancellationToken cancellationToken);
        Task<CreationUserData?> GetRegistrationDataByEmailAsync(string email, CancellationToken cancellationToken);
        Task<CreationUserData?> GetRegistrationDataByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task RemoveRegistrationDataAsync(Guid userId, CancellationToken cancellationToken);
        Task RemoveRegistrationDataByEmailAsync(string email, CancellationToken cancellationToken);


        Task SaveResetPasswordDataAsync(ResetPasswordData resetPasswordData, CancellationToken cancellationToken);
        Task<ResetPasswordData?> GetResetPasswordDataAsync(Guid userId, CancellationToken cancellationToken);
        Task RemoveResetPasswordDataAsync(Guid userId, CancellationToken cancellationToken);


        Task SaveChangeEmailDataAsync(ChangeEmailData changeEmailData, CancellationToken cancellationToken);
        Task<ChangeEmailData?> GetChangeEmailDataAsync(Guid userId, CancellationToken cancellationToken);
        Task RemoveChangeEmailDataAsync(Guid userId, CancellationToken cancellationToken);
    }
}