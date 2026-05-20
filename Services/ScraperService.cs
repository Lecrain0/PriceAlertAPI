using HtmlAgilityPack;
using PriceAlertAPI.Services.Interfaces;

namespace PriceAlertAPI.Services;

public class ScraperService : IScraperService
{
    private readonly HttpClient _http;
    private readonly ILogger<ScraperService> _logger;

    public ScraperService(HttpClient http, ILogger<ScraperService> logger)
    {
        _http = http;
        _logger = logger;

        // Bazı siteler bot isteğini reddeder, gerçek browser gibi görünelim
        _http.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36");
    }

    public async Task<decimal?> ScrapePriceAsync(string url)
    {
        try
        {
            var html = await _http.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Her site için XPath seçici — siteye göre genişlet
            var selectors = GetSelectorsForUrl(url);

            foreach (var selector in selectors)
            {
                var node = doc.DocumentNode.SelectSingleNode(selector);
                if (node == null) continue;

                var raw = node.InnerText
                    .Trim()
                    .Replace("TL", "")
                    .Replace("₺", "")
                    .Replace("\u00a0", "")  // non-breaking space
                    .Replace(".", "")
                    .Replace(",", ".")
                    .Trim();

                if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var price))
                {
                    _logger.LogInformation("Fiyat scraped: {Price} - URL: {Url}", price, url);
                    return price;
                }
            }

            _logger.LogWarning("Fiyat bulunamadı: {Url}", url);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scraping hatası: {Url}", url);
            return null;
        }
    }

    private static string[] GetSelectorsForUrl(string url)
    {
        // Desteklenen siteler — yeni site eklemek için buraya selector ekle
        if (url.Contains("trendyol.com"))
            return ["//span[contains(@class,'prc-dsc')]", "//span[contains(@class,'price')]"];

        if (url.Contains("hepsiburada.com"))
            return ["//span[@id='offering-price']", "//span[contains(@class,'price-value')]"];

        if (url.Contains("amazon.com.tr"))
            return ["//span[@class='a-price-whole']", "//span[contains(@class,'a-offscreen')]"];

        // Genel fallback
        return
        [
            "//span[contains(@class,'price')]",
            "//div[contains(@class,'price')]",
            "//span[contains(@itemprop,'price')]"
        ];
    }
}
