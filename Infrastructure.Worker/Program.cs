using Infrastructure.Data; 
using Microsoft.EntityFrameworkCore;
using Application.Interfaces; 
using Application.Services;
using Core.Entities; 
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(connectionString)
                    );

                    services.AddScoped<IApplicationDbContext>(sp =>
                        sp.GetRequiredService<AppDbContext>());

                    services.AddScoped<OfflineDetectionService>();

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}