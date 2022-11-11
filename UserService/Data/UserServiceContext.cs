using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;

namespace UserService.Data
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]

    public class UserServiceContext : DbContext
    {
        public UserServiceContext(DbContextOptions<UserServiceContext> options): base(options)
        {
        }

        public DbSet<UserService.Entities.User> User { get; set; }

        private string GetDebuggerDisplay()
        {
            return base.Database.ProviderName;
        }
    }
}
