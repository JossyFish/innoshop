using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
    public class ChangeEmailData
    {
        public Guid Id { get; }
        public string OldEmail { get; }
        public string NewEmail { get; }
        public string ConfirmationCode { get; }
        public string LinkToken { get; }

        public ChangeEmailData(Guid id, string oldEmail, string newEmail, string confirmationCode, string linkToken)
        {
            Id = id;
            OldEmail = oldEmail;
            NewEmail = newEmail;
            ConfirmationCode = confirmationCode;
            LinkToken = linkToken;
        }
    }
}
