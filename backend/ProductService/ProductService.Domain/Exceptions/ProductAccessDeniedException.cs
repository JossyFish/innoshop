using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Exceptions
{
    public class ProductAccessDeniedException : Exception
    {
        public Guid ProductId { get; }
        public Guid UserId { get; }

        public ProductAccessDeniedException(Guid productId, Guid userId)
            : base($"Access denied to product {productId} for user {userId}")
        {
            ProductId = productId;
            UserId = userId;
        }
    }
}
