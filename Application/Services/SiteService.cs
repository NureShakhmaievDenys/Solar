using Application.DTOs.Devices;
using Application.DTOs.Sites;
using Application.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SiteService
    {
        private readonly IApplicationDbContext _context;

        public SiteService(IApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<SiteDto> CreateSiteAsync(CreateSiteRequestDto request, Guid userId)
        {
            var site = new Site
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Location = request.Location,
                UserId = userId
            };
            _context.Sites.Add(site);
            await _context.SaveChangesAsync();
            return new SiteDto { Id = site.Id, Name = site.Name, Location = site.Location, UserId = site.UserId };
        }

        public async Task<List<SiteDto>> GetSitesForUserAsync(Guid userId)
        {
            return await _context.Sites
                .Where(s => s.UserId == userId)
                .Select(s => new SiteDto { Id = s.Id, Name = s.Name, Location = s.Location, UserId = s.UserId })
                .ToListAsync();
        }

        public async Task<SiteDto?> UpdateSiteAsync(Guid siteId, UpdateSiteRequestDto request, Guid userId)
        {
            var site = await _context.Sites
                .FirstOrDefaultAsync(s => s.Id == siteId && s.UserId == userId);

            if (site == null) return null; 
            site.Name = request.Name;
            site.Location = request.Location;

            await _context.SaveChangesAsync();

            return new SiteDto { Id = site.Id, Name = site.Name, Location = site.Location, UserId = site.UserId };
        }

        public async Task<bool> DeleteSiteAsync(Guid siteId, Guid userId)
        {
            var site = await _context.Sites
                .FirstOrDefaultAsync(s => s.Id == siteId && s.UserId == userId);

            if (site == null) return false; 

            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DeviceDto?> AddDeviceToSiteAsync(CreateDeviceRequestDto request, Guid siteId, Guid userId)
        {
            var siteExists = await _context.Sites.AnyAsync(s => s.Id == siteId && s.UserId == userId);
            if (!siteExists) return null;

            var device = new Device
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Brand = request.Brand,
                Config = request.Config?.RootElement.ToString(), 
                SiteId = siteId
            };
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return new DeviceDto { Id = device.Id, Name = device.Name, Brand = device.Brand, SiteId = device.SiteId };
        }

        public async Task<DeviceDto?> UpdateDeviceAsync(Guid siteId, Guid deviceId, UpdateDeviceRequestDto request, Guid userId)
        {
            var device = await _context.Devices
                .Include(d => d.Site)
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.SiteId == siteId && d.Site.UserId == userId);

            if (device == null) return null;

            device.Name = request.Name;
            device.Brand = request.Brand;
            device.Config = request.Config?.RootElement.ToString(); 

            await _context.SaveChangesAsync();

            return new DeviceDto { Id = device.Id, Name = device.Name, Brand = device.Brand, SiteId = device.SiteId };
        }

        public async Task<bool> DeleteDeviceAsync(Guid siteId, Guid deviceId, Guid userId)
        {
            var device = await _context.Devices
                .Include(d => d.Site)
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.SiteId == siteId && d.Site.UserId == userId);

            if (device == null) return false;

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}