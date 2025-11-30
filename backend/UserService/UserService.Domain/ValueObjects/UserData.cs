using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.ValueObjects
{
    public class UserData
    {
        public Guid SessionId { get; }          
        public string Email { get; }
        public string PasswordHash { get; }
        public string ConfirmationCode { get; }
        public DateTime ExpiresAt { get; }

        public UserData(string email, string passwordHash, string confirmationCode, TimeSpan expiresIn)
        {
            SessionId = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            ConfirmationCode = confirmationCode;
            ExpiresAt = DateTime.UtcNow.Add(expiresIn);
        }

    }


}
