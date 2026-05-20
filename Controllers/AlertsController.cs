using Microsoft.AspNetCore.Mvc;
using PriceAlertAPI.DTOs;
using PriceAlertAPI.Services.Interfaces;

namespace PriceAlertAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService) => _alertService = alertService;

    // GET api/alerts
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var alerts = await _alertService.GetAllAlertsAsync();
        return Ok(alerts);
    }

    // GET api/alerts/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var alert = await _alertService.GetAlertByIdAsync(id);
        if (alert == null) return NotFound(new { message = "Alert bulunamadı." });
        return Ok(alert);
    }

    // POST api/alerts
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAlertDto dto)
    {
        var created = await _alertService.CreateAlertAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DELETE api/alerts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _alertService.DeleteAlertAsync(id);
        return NoContent();
    }

    // POST api/alerts/trigger-check  (manuel test için)
    [HttpPost("trigger-check")]
    public async Task<IActionResult> TriggerCheck()
    {
        await _alertService.CheckAllAlertsAsync();
        return Ok(new { message = "Fiyat kontrolü manuel olarak çalıştırıldı." });
    }
}
