using Microsoft.EntityFrameworkCore;
using PriceAlertAPI.Data;
using PriceAlertAPI.DTOs;
using PriceAlertAPI.Models;
using PriceAlertAPI.Services.Interfaces;

namespace PriceAlertAPI.Services;

public class AlertService : IAlertService
{
    private readonly AppDbContext _db;
    private readonly IScraperService _scraper;
    private readonly INotificationService _notification;
    private readonly ILogger<AlertService> _logger;

    public AlertService(
        AppDbContext db,
        IScraperService scraper,
        INotificationService notification,
        ILogger<AlertService> logger)
    {
        _db = db;
        _scraper = scraper;
        _notification = notification;
        _logger = logger;
    }

    public async Task<AlertResponseDto> CreateAlertAsync(CreateAlertDto dto)
    {
        // Aynı URL için ürün zaten varsa tekrar oluşturma
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Url == dto.Url);

        if (product == null)
        {
            var scrapedPrice = await _scraper.ScrapePriceAsync(dto.Url) ?? 0;

            product = new Product
            {
                Url = dto.Url,
                Name = dto.ProductName,
                CurrentPrice = scrapedPrice,
                LastCheckedAt = DateTime.UtcNow
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            // İlk fiyat geçmişi kaydı
            if (scrapedPrice > 0)
                await AddPriceHistoryAsync(product.Id, scrapedPrice);
        }

        var alert = new PriceAlert
        {
            ProductId = product.Id,
            TargetPrice = dto.TargetPrice,
            NotifyEmail = dto.NotifyEmail
        };

        _db.PriceAlerts.Add(alert);
        await _db.SaveChangesAsync();

        return MapToDto(alert, product);
    }

    public async Task<IEnumerable<AlertResponseDto>> GetAllAlertsAsync()
    {
        var alerts = await _db.PriceAlerts
            .Include(a => a.Product)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return alerts.Select(a => MapToDto(a, a.Product));
    }

    public async Task<AlertResponseDto?> GetAlertByIdAsync(int id)
    {
        var alert = await _db.PriceAlerts
            .Include(a => a.Product)
            .FirstOrDefaultAsync(a => a.Id == id);

        return alert == null ? null : MapToDto(alert, alert.Product);
    }

    public async Task DeleteAlertAsync(int id)
    {
        var alert = await _db.PriceAlerts.FindAsync(id)
            ?? throw new KeyNotFoundException($"Alert bulunamadı: {id}");

        _db.PriceAlerts.Remove(alert);
        await _db.SaveChangesAsync();
    }

    public async Task CheckAllAlertsAsync()
    {
        var products = await _db.Products
            .Include(p => p.Alerts.Where(a => !a.IsTriggered))
            .Where(p => p.Alerts.Any(a => !a.IsTriggered))
            .ToListAsync();

        foreach (var product in products)
        {
            var newPrice = await _scraper.ScrapePriceAsync(product.Url);
            if (newPrice == null) continue;

            if (newPrice != product.CurrentPrice)
            {
                product.CurrentPrice = newPrice.Value;
                product.LastCheckedAt = DateTime.UtcNow;
                await AddPriceHistoryAsync(product.Id, newPrice.Value);
            }

            foreach (var alert in product.Alerts)
            {
                if (product.CurrentPrice <= alert.TargetPrice)
                {
                    _logger.LogInformation("Fiyat alarmı tetiklendi! Ürün: {Name}, Fiyat: {Price}",
                        product.Name, product.CurrentPrice);

                    await _notification.SendPriceAlertEmailAsync(
                        alert.NotifyEmail,
                        product.Name,
                        alert.TargetPrice,
                        product.CurrentPrice);

                    alert.IsTriggered = true;
                }
            }
        }

        await _db.SaveChangesAsync();
    }

    private async Task AddPriceHistoryAsync(int productId, decimal price)
    {
        _db.PriceHistories.Add(new PriceHistory
        {
            ProductId = productId,
            Price = price,
            RecordedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    private static AlertResponseDto MapToDto(PriceAlert alert, Product product) => new()
    {
        Id = alert.Id,
        ProductName = product.Name,
        ProductUrl = product.Url,
        CurrentPrice = product.CurrentPrice,
        TargetPrice = alert.TargetPrice,
        NotifyEmail = alert.NotifyEmail,
        IsTriggered = alert.IsTriggered,
        CreatedAt = alert.CreatedAt
    };
}
