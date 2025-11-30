using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Exceptions
{
    public class ConfirmCodeExpiredException : Exception
    {
        public string ConfirmCode { get; }

        public ConfirmCodeExpiredException(string confirmCode)
            : base($"ConfirmCode '{confirmCode}' is expired")
        {
            ConfirmCode = confirmCode;
        }
    }
}
