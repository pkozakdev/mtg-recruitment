using Microsoft.EntityFrameworkCore;
using macrix_api.EF;

namespace macrix_api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { 
        }

        public DbSet<User> Users => Set<User>();
    }
}
