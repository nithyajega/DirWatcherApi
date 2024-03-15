using DirWatcherApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DirWatcherApi.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<TaskRun> TaskRuns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer(optionsBuilder.Options.FindExtension<Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal.SqlServerOptionsExtension>().ConnectionString);
                optionsBuilder.UseSqlServer("DirWatcherConnectionString");
            }

        }
       
    }

}
