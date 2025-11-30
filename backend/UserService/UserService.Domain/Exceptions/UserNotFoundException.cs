using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public string Email { get; }

        public UserNotFoundException(string email)
            : base($"User with email '{email}' not found")
        {
            Email = email;
        }
    }
}
