# Bu proje, örnek bir ekosistem olarak aktif olarak geliştirilmektedir. Katkılarınız ve geri bildirimleriniz memnuniyetle karşılanacaktır!


# 🔔 PriceAlertAPI

Ürün fiyatlarını takip eden ve hedef fiyata ulaşıldığında e-posta bildirimi gönderen bir ASP.NET Core Web API'si.

## Özellikler

- 🕷️ Trendyol, Hepsiburada, Amazon.com.tr fiyat takibi
- 📉 Hedef fiyata ulaşıldığında otomatik e-posta bildirimi
- 📊 Ürün fiyat geçmişi
- ⏰ Arka planda periyodik fiyat kontrolü (BackgroundService)
- 🛡️ Global hata yönetimi (Middleware)

## Teknolojiler

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core + SQLite
- HtmlAgilityPack (web scraping)
- MailKit (e-posta gönderimi)

## Kurulum

```bash
git clone https://github.com/Lecrain0/PriceAlertAPI.git
cd PriceAlertAPI
dotnet restore
dotnet run
```

### E-posta Ayarları

`appsettings.json` içindeki `Email` bölümünü doldurun.
Gmail kullanıyorsanız [App Password](https://myaccount.google.com/apppasswords) oluşturmanız gerekir.

## API Endpoints

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/alerts` | Tüm alarmları listele |
| GET | `/api/alerts/{id}` | Alarm detayı |
| POST | `/api/alerts` | Yeni alarm oluştur |
| DELETE | `/api/alerts/{id}` | Alarm sil |
| POST | `/api/alerts/trigger-check` | Manuel fiyat kontrolü |
| GET | `/api/products` | Takip edilen ürünler |
| GET | `/api/products/{id}/history` | Ürün fiyat geçmişi |


> Not: Trendyol bot koruması nedeniyle fiyat çekimi
> zaman zaman çalışmayabilir. PuppeteerSharp entegrasyonu roadmap'tedir.

### Örnek İstek

```json
POST /api/alerts
{
  "url": "https://www.trendyol.com/urun-linki",
  "productName": "Kablosuz Kulaklık",
  "targetPrice": 800.00,
  "notifyEmail": "ornek@gmail.com"
}
```

## Desteklenen Siteler

- ✅ Trendyol
- ✅ Hepsiburada  
- ✅ Amazon.com.tr
- 🔧 Genel HTML fallback (diğer siteler)

## Lisans

MIT
