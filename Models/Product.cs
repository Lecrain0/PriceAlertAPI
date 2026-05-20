namespace PriceAlertAPI.Models;

public class Product
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public DateTime LastCheckedAt { get; set; }
    public ICollection<PriceAlert> Alerts { get; set; } = new List<PriceAlert>();
}
