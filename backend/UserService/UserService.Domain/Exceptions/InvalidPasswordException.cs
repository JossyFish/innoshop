using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Exceptions
{
    public class InvalidPasswordException : Exception
    {
        public string Password { get; }

        public InvalidPasswordException(string password)
            : base($"Invalid password '{password}'")
        {
            Password = password;
        }
    }
}
