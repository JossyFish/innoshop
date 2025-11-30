using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Exceptions
{
    public class ProductUserNotFoundException : Exception
    {
        public Guid Id { get; }

        public ProductUserNotFoundException(Guid id)
            : base($"User with id '{id}' not found")
        {
            Id = id;
        }
    }
}
