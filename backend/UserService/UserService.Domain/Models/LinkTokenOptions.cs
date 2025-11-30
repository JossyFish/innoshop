using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
    public class LinkTokenOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiresMinutes { get; set; }
    }
}
