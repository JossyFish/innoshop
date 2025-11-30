using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public Guid Id { get; }

        public ProductNotFoundException(Guid id)
            : base($"Product with id '{id}' not found")
        {
            Id = id;
        }
    }
}
