namespace PriceAlertAPI.DTOs;

public class CreateAlertDto
{
    public string Url { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal TargetPrice { get; set; }
    public string NotifyEmail { get; set; } = string.Empty;
}

public class AlertResponseDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductUrl { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal TargetPrice { get; set; }
    public string NotifyEmail { get; set; } = string.Empty;
    public bool IsTriggered { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public DateTime LastCheckedAt { get; set; }
}

public class PriceHistoryDto
{
    public decimal Price { get; set; }
    public DateTime RecordedAt { get; set; }
}
