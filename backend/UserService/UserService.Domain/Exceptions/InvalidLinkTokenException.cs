using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Exceptions
{
    public class InvalidLinkTokenException : Exception
    {
        public InvalidLinkTokenException(string message) : base(message)
        {
        }
    }
}
