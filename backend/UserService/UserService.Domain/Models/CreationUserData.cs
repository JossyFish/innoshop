using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
    public class CreationUserData
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Email { get; }
        public string PasswordHash { get; }
        public string ConfirmationCode { get; }
        public string LinkToken { get; }

        public CreationUserData(Guid id, string name, string email, string passwordHash, string confirmationCode, string linkToken)
        {
            Id = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            ConfirmationCode = confirmationCode;
            LinkToken = linkToken;
        }

    }
}
