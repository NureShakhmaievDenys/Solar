using Application.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OfflineDetectionService
    {
        private readonly IApplicationDbContext _context;
        private const int OFFLINE_THRESHOLD_MINUTES = 10;
        private const string OFFLINE_MESSAGE_PREFIX = "Критическое оповещение: Устройство";

        public OfflineDetectionService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CheckDeviceStatusesAsync()
        {
            var threshold = DateTime.UtcNow.AddMinutes(-OFFLINE_THRESHOLD_MINUTES);

            var allDevices = await _context.Devices
                .Include(d => d.Site)
                .Select(d => new
                {
                    Device = d,
                    IsOffline = !d.TelemetryData.Any() || d.TelemetryData.All(t => t.Timestamp < threshold),
                    UserId = d.Site.UserId
                })
                .ToListAsync();

            var activeOfflineAlerts = await _context.Notifications
                .Where(n => !n.IsRead && n.Message.StartsWith(OFFLINE_MESSAGE_PREFIX))
                .ToListAsync();

            var notificationsToCreate = new List<Notification>();
            var alertsToResolve = new List<Notification>();

            foreach (var deviceStatus in allDevices)
            {
                var existingAlert = activeOfflineAlerts
                    .FirstOrDefault(a => a.UserId == deviceStatus.UserId && a.Message.Contains($"'{deviceStatus.Device.Name}'"));

                if (deviceStatus.IsOffline)
                {
                    if (existingAlert == null)
                    {
                        notificationsToCreate.Add(new Notification
                        {
                            Id = Guid.NewGuid(),
                            UserId = deviceStatus.UserId,
                            Message = $"{OFFLINE_MESSAGE_PREFIX} '{deviceStatus.Device.Name}' не выходило на связь более {OFFLINE_THRESHOLD_MINUTES} минут.",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    if (existingAlert != null)
                    {
                        existingAlert.IsRead = true;
                        alertsToResolve.Add(existingAlert);
                    }
                }
            }

            if (notificationsToCreate.Any())
            {
                await _context.Notifications.AddRangeAsync(notificationsToCreate);
            }

            if (notificationsToCreate.Any() || alertsToResolve.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}