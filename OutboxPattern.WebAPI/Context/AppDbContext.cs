using Microsoft.EntityFrameworkCore;
using OutboxPattern.WebAPI.Configrution;
using OutboxPattern.WebAPI.Models;

namespace OutboxPattern.WebAPI.Context
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        }


    }
}
