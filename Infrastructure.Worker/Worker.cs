using Application.Services;

namespace Infrastructure.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        private const int CHECK_INTERVAL_MINUTES = 1;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Offline Detection Worker запущен в: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var detectionService = scope.ServiceProvider
                            .GetRequiredService<OfflineDetectionService>();

                        _logger.LogInformation("Перевірка офлайн-пристрою...");
                        await detectionService.CheckDeviceStatusesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Помилка в офлайн пристрої");
                }
                await Task.Delay(TimeSpan.FromMinutes(CHECK_INTERVAL_MINUTES), stoppingToken);
            }
        }
    }
}
