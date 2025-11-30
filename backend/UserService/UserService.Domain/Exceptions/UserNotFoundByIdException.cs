using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Exceptions
{
    public class UserNotFoundByIdException : Exception
    {
        public Guid Id { get; }

        public UserNotFoundByIdException(Guid id)
            : base($"User with id '{id}' not found")
        {
            Id = id;
        }
    }
}
