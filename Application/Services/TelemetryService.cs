// Application/Services/TelemetryService.cs
using Application.DTOs.Telemetry;
using Application.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TelemetryService
    {
        private readonly IApplicationDbContext _context;

        public TelemetryService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IngestTelemetryAsync(Guid deviceId, TelemetryIngestDto data)
        {
            var deviceExists = await _context.Devices.AnyAsync(d => d.Id == deviceId);
            if (!deviceExists)
            {
                return false;
            }

            var telemetryData = new TelemetryData
            {
                DeviceId = deviceId,
                Timestamp = data.Timestamp ?? DateTime.UtcNow,
                GenerationWatts = data.GenerationWatts,
                ConsumptionWatts = data.ConsumptionWatts,
                GridImportWatts = data.GridImportWatts,
                GridExportWatts = data.GridExportWatts,
                BatteryPercent = data.BatteryPercent
            };

            _context.TelemetryData.Add(telemetryData);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}