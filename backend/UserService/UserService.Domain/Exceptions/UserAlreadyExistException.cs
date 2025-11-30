using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Exceptions
{
    public class UserAlreadyExistException : Exception
    {
        public string Email { get; }

        public UserAlreadyExistException(string email)
            : base($"User with email '{email}' already exists")
        {
            Email = email;
        }
    }
}
