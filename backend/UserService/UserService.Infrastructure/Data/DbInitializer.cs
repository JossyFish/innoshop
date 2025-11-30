using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data
{
    public class DbInitializer
    {
        UserDBContext _context;

        public DbInitializer(UserDBContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var anyUsersExist = await _context.Users.AnyAsync();

            if (!anyUsersExist)
            {
                var adminUser = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "admin",
                    Email = "admin@innoshop.com",
                    PasswordHash = "$2a$11$KPlU/rnU29hasIMdD0tIT.P7bMf5yJVEd0t1v9Zj.SmE3yshjPTtu",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(adminUser);

                var adminRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == 1); 

                if (adminRole != null)
                {
                    adminUser.Roles.Add(adminRole);
                }


                var exampleUser = new UserEntity
                {
                    Id = Guid.Parse("b9ed240f-be97-47a9-b00f-b99a96320e28"),
                    Name = "example",
                    Email = "example@gmail.com",
                    PasswordHash = "$2a$11$KPlU/rnU29hasIMdD0tIT.P7bMf5yJVEd0t1v9Zj.SmE3yshjPTtu",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(exampleUser);

                var exapleUserRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == 2);

                if (exapleUserRole != null)
                {
                    exampleUser.Roles.Add(exapleUserRole);
                }


                await _context.SaveChangesAsync();
            }
        }
    }
}
