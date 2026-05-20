namespace PriceAlertAPI.Models;

public class PriceAlert
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal TargetPrice { get; set; }
    public string NotifyEmail { get; set; } = string.Empty;
    public bool IsTriggered { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
