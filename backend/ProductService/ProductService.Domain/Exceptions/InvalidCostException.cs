using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Exceptions
{
    public class InvalidCostException : Exception
    {
        public decimal Cost { get; }

        public InvalidCostException(decimal cost)
            : base($"Invalid cost '{cost}' for product")
        {
            Cost = cost;
        }
    }
}
