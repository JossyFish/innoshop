using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
    public class User
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get;}
        public List<string> Roles { get; }

        public User(Guid id, string name, string email, string passwordHash, bool isActive, DateTime createdAt)
    : this(id, name, email, passwordHash, isActive, createdAt, null)
        {
        }


        public User(Guid id, string name, string email, string passwordHash, bool isActive, 
            DateTime createdAt, List<string> roles)
        {
            Id = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            IsActive = isActive;
            CreatedAt = createdAt;
            Roles = roles ?? new List<string>();
        }
        public static User Create(Guid id, string name, string email, string passwordHash, bool isActive, DateTime createdAt)
        {
            return new User(id, name, email, passwordHash, isActive, createdAt);
        }

        public static User CreateWithRoles(Guid id, string name, string email, string passwordHash,
                                       bool isActive, DateTime createdAt, List<string> roles)
        {
            return new User(id, name, email, passwordHash, isActive, createdAt, roles);
        }
        public void ChangeName(string newName)
        {
            Name = newName;
        }
        public void ChangeEmail(string newEmail)
        {
            Email = newEmail;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        public void ChangeActive(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
