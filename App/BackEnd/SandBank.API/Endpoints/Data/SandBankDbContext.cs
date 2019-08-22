using Domain;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Data
{
    public class SandBankDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public SandBankDbContext(DbContextOptions<SandBankDbContext> options)
            : base(options)
        {
            
        }
    }
}