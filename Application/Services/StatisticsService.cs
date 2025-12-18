using Application.DTOs.Statistics;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class StatisticsService
    {
        private readonly IApplicationDbContext _context;

        public StatisticsService(IApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. АДМІНІСТРУВАННЯ ---
        public async Task<SystemOverviewDto> GetSystemOverviewAsync()
        {
            var yesterday = DateTime.UtcNow.AddHours(-24);

            return new SystemOverviewDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalSites = await _context.Sites.CountAsync(),
                ActiveDevices = await _context.Devices
                    .Where(d => d.TelemetryData.Any(t => t.Timestamp >= yesterday))
                    .CountAsync(),
                TotalTelemetryRecords = await _context.TelemetryData.LongCountAsync()
            };
        }

        public async Task<StatisticsResponseDto?> GetDailyStatisticsAsync(
            Guid siteId, 
            Guid userId, 
            DateTime startDate, 
            DateTime endDate, 
            double tariffPrice)
        {
            var siteExists = await _context.Sites.AnyAsync(s => s.Id == siteId && s.UserId == userId);
            if (!siteExists) return null;
            var deviceIds = await _context.Devices
                .Where(d => d.SiteId == siteId)
                .Select(d => d.Id)
                .ToListAsync();

            if (!deviceIds.Any()) return new StatisticsResponseDto();
            var dailyData = await _context.TelemetryData
                .Where(t => deviceIds.Contains(t.DeviceId) && t.Timestamp >= startDate && t.Timestamp <= endDate)
                .GroupBy(t => t.Timestamp.Date)
                .Select(g => new 
                {
                    Date = g.Key,
                    AvgGenWatts = g.Average(x => x.GenerationWatts),
                    AvgConsWatts = g.Average(x => x.ConsumptionWatts)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
            var dailyStatsList = dailyData.Select(d => 
            {
                double genWh = d.AvgGenWatts * 24;
                double consWh = d.AvgConsWatts * 24;
                double moneySaved = (genWh / 1000.0) * tariffPrice;
                
                double selfSufficiency = 0;
                if (consWh > 0)
                {
                    selfSufficiency = genWh >= consWh ? 100 : (genWh / consWh) * 100;
                }

                // Чистий баланс (Експорт - Імпорт)
                double exportWh = (genWh > consWh) ? (genWh - consWh) : 0;
                double importWh = (consWh > genWh) ? (consWh - genWh) : 0;
                double netBalance = exportWh - importWh;

                return new DailyStatisticsDto
                {
                    Date = d.Date,
                    TotalGenerationWh = Math.Round(genWh, 2),
                    TotalConsumptionWh = Math.Round(consWh, 2),
                    MoneySaved = Math.Round(moneySaved, 2),
                    SelfSufficiencyPercent = Math.Round(selfSufficiency, 1),
                    NetGridBalanceWh = Math.Round(netBalance, 2)
                };
            }).ToList();

            // 5. Розрахунок загальних підсумків (Totals)
            // Якщо список порожній, повертаємо нулі
            var totals = new DailyStatisticsDto
            {
                Date = startDate, 
                TotalGenerationWh = Math.Round(dailyStatsList.Sum(x => x.TotalGenerationWh), 2),
                TotalConsumptionWh = Math.Round(dailyStatsList.Sum(x => x.TotalConsumptionWh), 2),
                MoneySaved = Math.Round(dailyStatsList.Sum(x => x.MoneySaved), 2),
                NetGridBalanceWh = Math.Round(dailyStatsList.Sum(x => x.NetGridBalanceWh), 2),
                
                // Середній відсоток автономності за період
                SelfSufficiencyPercent = dailyStatsList.Any() 
                    ? Math.Round(dailyStatsList.Average(x => x.SelfSufficiencyPercent), 1) 
                    : 0
            };
           
            return new StatisticsResponseDto
            {
                Totals = totals,
                DailyStats = dailyStatsList
            };
        }
    }
}
//2025-12-04T00:00:00Z