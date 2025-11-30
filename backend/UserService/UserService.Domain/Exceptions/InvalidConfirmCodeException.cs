using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Exceptions
{
    public class InvalidConfirmCodeException : Exception
    {
        public string ConfirmCode { get; }

        public InvalidConfirmCodeException(string confirmCode)
            : base($"Invalid confirmCode '{confirmCode}'")
        {
            ConfirmCode = confirmCode;
        }
    }
}
