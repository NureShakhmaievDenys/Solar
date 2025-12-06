using Application.DTOs.Dashboard;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DashboardService
    {
        private readonly IApplicationDbContext _context;

        public DashboardService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RealtimeDashboardDto?> GetRealtimeDataAsync(Guid siteId, Guid userId)
        {
            var site = await _context.Sites
                .AsNoTracking() 
                .FirstOrDefaultAsync(s => s.Id == siteId && s.UserId == userId);

            if (site == null)
            {
                return null; 
            }

            var deviceIds = await _context.Devices
                .Where(d => d.SiteId == siteId)
                .Select(d => d.Id)
                .ToListAsync();

            if (!deviceIds.Any())
            {
                return new RealtimeDashboardDto { SiteId = siteId, Timestamp = DateTime.UtcNow };
            }
            var latestData = await _context.TelemetryData
                .AsNoTracking()
                .Where(t => deviceIds.Contains(t.DeviceId))
                .OrderByDescending(t => t.Timestamp) 
                .FirstOrDefaultAsync();

            if (latestData == null)
            {
                return new RealtimeDashboardDto { SiteId = siteId, Timestamp = DateTime.UtcNow };
            }

            return new RealtimeDashboardDto
            {
                SiteId = siteId,
                Timestamp = latestData.Timestamp,
                GenerationWatts = latestData.GenerationWatts,
                ConsumptionWatts = latestData.ConsumptionWatts,
                GridImportWatts = latestData.GridImportWatts,
                GridExportWatts = latestData.GridExportWatts,
                BatteryPercent = latestData.BatteryPercent
            };
        }
    }
}