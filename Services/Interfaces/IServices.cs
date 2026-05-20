using PriceAlertAPI.DTOs;

namespace PriceAlertAPI.Services.Interfaces;

public interface IAlertService
{
    Task<AlertResponseDto> CreateAlertAsync(CreateAlertDto dto);
    Task<IEnumerable<AlertResponseDto>> GetAllAlertsAsync();
    Task<AlertResponseDto?> GetAlertByIdAsync(int id);
    Task DeleteAlertAsync(int id);
    Task CheckAllAlertsAsync();
}

public interface IScraperService
{
    Task<decimal?> ScrapePriceAsync(string url);
}

public interface INotificationService
{
    Task SendPriceAlertEmailAsync(string toEmail, string productName, decimal targetPrice, decimal currentPrice);
}
