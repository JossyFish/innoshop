using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Exceptions
{
    public class InvalidCurrencyException : Exception
    {
        public string Currency { get; }

        public InvalidCurrencyException(string currency)
            : base($"Invalid currency '{currency}' for product")
        {
            Currency = currency;
        }
    }
}
