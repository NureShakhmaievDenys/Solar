using Application.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<TelemetryData> TelemetryData { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                 .HasMany(u => u.Sites) 
                 .WithOne(s => s.User) 
                 .HasForeignKey(s => s.UserId) 
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TelemetryData>()
                .HasIndex(t => new { t.DeviceId, t.Timestamp })
                .HasDatabaseName("IX_TelemetryData_Device_Timestamp");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}