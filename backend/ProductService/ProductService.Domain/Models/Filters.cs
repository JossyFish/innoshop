using ProductService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Models
{
    public class Filters
    {
        public string? Name { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public Currency? Currency { get; set; }
        public int? MinQuantity { get; init; }
        public List<Guid>? UserIds { get; set; }
        public DateTime? MinCreatedAt { get; init; }
        public DateTime? MaxCreatedAt { get; init; }
    }
}
