using Microsoft.EntityFrameworkCore;
using NB_Project.Model;

namespace NB_Project.ApplicationDbContext
{
    public class DB:DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(e => e.Email).IsUnique();
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(e => e.phoneNumber).IsUnique();
        }

        public DbSet<User> users { get; set; }    
      public DbSet<Car> cars { get; set; }
      public DbSet<ChatMessage> chatMessages { get; set; }
      public DbSet<ChatSummary> chatSummaries { get; set; }
       

    }
}
