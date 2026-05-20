using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceAlertAPI.Data;
using PriceAlertAPI.DTOs;

namespace PriceAlertAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db) => _db = db;

    // GET api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _db.Products
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Url = p.Url,
                CurrentPrice = p.CurrentPrice,
                LastCheckedAt = p.LastCheckedAt
            })
            .ToListAsync();

        return Ok(products);
    }

    // GET api/products/5/history
    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetHistory(int id)
    {
        var exists = await _db.Products.AnyAsync(p => p.Id == id);
        if (!exists) return NotFound(new { message = "Ürün bulunamadı." });

        var history = await _db.PriceHistories
            .Where(h => h.ProductId == id)
            .OrderByDescending(h => h.RecordedAt)
            .Select(h => new PriceHistoryDto
            {
                Price = h.Price,
                RecordedAt = h.RecordedAt
            })
            .ToListAsync();

        return Ok(history);
    }
}
