using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using telegram_bot.Models.Register.Request;
using telegram_bot.Models.Register.Response;
using telegram_bot.Models.Users;


namespace telegram_bot.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions <Context> options) : base(options)
        {
        }

        public DbSet<RegisterRequest> RegisterRequest { get; set; }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RegisterRequest>()
                .HasIndex(u => u.MobileNumber)
                .IsUnique(); 
        }
        
        
    }
}
