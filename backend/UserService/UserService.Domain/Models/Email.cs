using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
    public class Email
    {
        public string UserEmail {  get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;

        public Email(string userEmail, string subject, string body, bool isHtml = true)
        {
            UserEmail = userEmail;
            Subject = subject;
            Body = body;
            IsHtml = isHtml;
        }
    }
}
