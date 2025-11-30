using ProductService.Domain.Enums;
using ProductService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Exceptions;

namespace ProductService.Domain.ValueObjects
{
    public record Price
    {
        public decimal Cost { get; set; }
        public Currency Currency { get; set;  }

        public Price() { }

        public Price(decimal cost, Currency currency = Currency.BYN)
        {
            if (cost < 0)
                throw new InvalidCostException(cost);

            Cost = cost;
            Currency = currency;
        }
    }
}
