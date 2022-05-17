using Microsoft.EntityFrameworkCore;

namespace DailyDo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base (options) { }
        public DbSet<Models.Task> Tasks { get; set; }

    }
  
}
