using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Exceptions
{
    public class InvalidOperation : Exception
    {
        public string Operation { get; }

        public InvalidOperation(string operation)
            : base($"Invalid operation '{operation}' for product")
        {
            Operation = operation;
        }
    }
}
