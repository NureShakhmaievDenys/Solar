// Application/Interfaces/IApplicationDbContext.cs
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }

        DbSet<Site> Sites { get; }

        DbSet<Device> Devices { get; }
        DbSet<TelemetryData> TelemetryData { get; }
        DbSet<Notification> Notifications { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}