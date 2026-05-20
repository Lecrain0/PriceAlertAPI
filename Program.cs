using Microsoft.EntityFrameworkCore;
using PriceAlertAPI.BackgroundServices;
using PriceAlertAPI.Data;
using PriceAlertAPI.Middlewares;
using PriceAlertAPI.Services;
using PriceAlertAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Veritabanı
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Servisler
builder.Services.AddHttpClient<IScraperService, ScraperService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();

// Arka plan servisi
builder.Services.AddHostedService<PriceCheckWorker>();

var app = builder.Build();

// Veritabanını otomatik oluştur
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
