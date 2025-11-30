using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;

namespace UserService.Infrastructure.Repositories
{
    public class CacheUsersRepository : ICacheUsersRepository
    {
        private readonly HybridCache _cache;
        private readonly ILogger<CacheUsersRepository> _logger;
        private readonly CacheOptions _options;

        public CacheUsersRepository(HybridCache cache, ILogger<CacheUsersRepository> logger, IOptions<CacheOptions> options)
        {
            _cache = cache;
            _logger = logger;
            _options = options.Value;
        }

        private HybridCacheEntryOptions UserCacheOptions => new()
        {
            LocalCacheExpiration = _options.UserLocalCacheExpiration,
            Expiration = _options.UserExpiration
        };

        private HybridCacheEntryOptions ResetPasswordCacheOptions => new()
        {
            LocalCacheExpiration = _options.ResetPasswordLocalCacheExpiration,
            Expiration = _options.ResetPasswordExpiration
        };

        private async Task SaveAsync<T>(string key, T data, HybridCacheEntryOptions options, CancellationToken cancellationToken)
        {
            try
            {
                await _cache.SetAsync(key, data, options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cache data with key: {Key}", key);
            }
        }

        private async Task<T?> GetAsync<T>(string key, HybridCacheEntryOptions options, CancellationToken cancellationToken)
        {
            try
            {
                return await _cache.GetOrCreateAsync<T>(
                    key,
                    async cancel => await Task.FromResult<T?>(default),
                    options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get data with key: {Key}", key);
                return default;
            }
        }



        public async Task SaveRegistrationDataAsync(CreationUserData creationUserData, CancellationToken cancellationToken)
        {
            var key = GetUserKey(creationUserData.Id);
            var emailKey = GetEmailKey(creationUserData.Email);

            await SaveAsync(key, creationUserData, UserCacheOptions, cancellationToken);
            await SaveAsync(emailKey, creationUserData.Id.ToString(), UserCacheOptions, cancellationToken);
        }

        public async Task<CreationUserData?> GetRegistrationDataByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var emailKey = GetEmailKey(email);
            var userIdStr = await GetAsync<string>(emailKey, UserCacheOptions, cancellationToken);

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return null;

            var userKey = GetUserKey(userId);
            return await GetAsync<CreationUserData>(userKey, UserCacheOptions, cancellationToken);
        }

        public async Task<CreationUserData?> GetRegistrationDataByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var key = GetUserKey(userId);
            return await GetAsync<CreationUserData>(key, UserCacheOptions, cancellationToken);
        }

        public async Task RemoveRegistrationDataAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetRegistrationDataByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                await _cache.RemoveAsync(GetUserKey(userId), cancellationToken);
                await _cache.RemoveAsync(GetEmailKey(user.Email), cancellationToken);
            }
        }

        public async Task RemoveRegistrationDataByEmailAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                var emailKey = GetEmailKey(email);
                var userIdStr = await GetAsync<string>(emailKey, UserCacheOptions, cancellationToken);

                await _cache.RemoveAsync(emailKey, cancellationToken);

                if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var userId))
                {
                    await _cache.RemoveAsync(GetUserKey(userId), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove registration data by email: {Email}", email);
            }
        }
        


        public async Task SaveResetPasswordDataAsync(ResetPasswordData resetPasswordData, CancellationToken cancellationToken)
        {
            var key = GetResetPasswordKey(resetPasswordData.Id);
            await SaveAsync(key, resetPasswordData, ResetPasswordCacheOptions, cancellationToken);
        }

        public async Task<ResetPasswordData?> GetResetPasswordDataAsync(Guid userId, CancellationToken cancellationToken)
        {
            var key = GetResetPasswordKey(userId);
            return await GetAsync<ResetPasswordData>(key, ResetPasswordCacheOptions, cancellationToken);
        }

        public async Task RemoveResetPasswordDataAsync(Guid userId, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(GetResetPasswordKey(userId), cancellationToken);
        }



        public async Task SaveChangeEmailDataAsync(ChangeEmailData changeEmailData, CancellationToken cancellationToken)
        {
            var key = GetChangeEmailKey(changeEmailData.Id);
            await SaveAsync(key, changeEmailData, ResetPasswordCacheOptions, cancellationToken);
        }

        public async Task<ChangeEmailData?> GetChangeEmailDataAsync(Guid userId, CancellationToken cancellationToken)
        {
            var key = GetChangeEmailKey(userId);
            return await GetAsync<ChangeEmailData>(key, ResetPasswordCacheOptions, cancellationToken);
        }

        public async Task RemoveChangeEmailDataAsync(Guid userId, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(GetChangeEmailKey(userId), cancellationToken);
        }


        private static string GetUserKey(Guid userId) => $"user_id:{userId}";
        private static string GetEmailKey(string email) => $"user_email:{email}";
        private static string GetResetPasswordKey(Guid userId) => $"reset_password:{userId}";
        private static string GetChangeEmailKey(Guid userId) => $"change_email:{userId}";
    }
}