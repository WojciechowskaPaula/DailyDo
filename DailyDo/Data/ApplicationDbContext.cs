using Microsoft.EntityFrameworkCore;

namespace DailyDo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base (options) { }
        DbSet<Task> Tasks { get; set; }
    }
}
