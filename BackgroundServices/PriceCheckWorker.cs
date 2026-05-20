using PriceAlertAPI.Services.Interfaces;

namespace PriceAlertAPI.BackgroundServices;

public class PriceCheckWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PriceCheckWorker> _logger;
    private readonly IConfiguration _config;

    public PriceCheckWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<PriceCheckWorker> logger,
        IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMinutes = _config.GetValue<int>("PriceCheck:IntervalMinutes", 30);

        _logger.LogInformation("PriceCheckWorker başladı. Kontrol aralığı: {Interval} dakika", intervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Fiyat kontrolü başlıyor... {Time}", DateTime.UtcNow);

                using var scope = _scopeFactory.CreateScope();
                var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();
                await alertService.CheckAllAlertsAsync();

                _logger.LogInformation("Fiyat kontrolü tamamlandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fiyat kontrolü sırasında hata oluştu.");
            }

            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
        }
    }
}
