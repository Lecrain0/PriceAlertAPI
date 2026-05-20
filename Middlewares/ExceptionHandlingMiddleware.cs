using System.Text.Json;

namespace PriceAlertAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Kaynak bulunamadı.");
            await WriteResponse(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Geçersiz argüman.");
            await WriteResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen hata.");
            await WriteResponse(context, StatusCodes.Status500InternalServerError,
                "Sunucuda beklenmeyen bir hata oluştu.");
        }
    }

    private static async Task WriteResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(
                System.Text.Unicode.UnicodeRanges.All),
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            error = message,
            statusCode,
            timestamp = DateTime.UtcNow
        }, options));
    }
}
