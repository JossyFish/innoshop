using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
    public class ResetPasswordData
    {
        public Guid Id { get; }
        public string Email { get; }
        public string ConfirmationCode { get; }

        public ResetPasswordData(Guid id, string email, string confirmationCode)
        {
            Id = id;
            Email = email;
            ConfirmationCode = confirmationCode;
        }
    }
}
