using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
    public class AppSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string FrontendUrl { get; set; } = string.Empty;
    }
}
