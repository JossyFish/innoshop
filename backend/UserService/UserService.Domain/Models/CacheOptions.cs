using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
    public class CacheOptions
    {
        public TimeSpan UserLocalCacheExpiration { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan UserExpiration { get; set; } = TimeSpan.FromMinutes(10);
        public TimeSpan ResetPasswordLocalCacheExpiration { get; set; }
        public TimeSpan ResetPasswordExpiration { get; set; }
    }
}
