namespace PriceAlertAPI.Models;

public class PriceHistory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
