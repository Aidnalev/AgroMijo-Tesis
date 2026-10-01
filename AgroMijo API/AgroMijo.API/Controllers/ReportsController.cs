using AgroMijo.API.Data;
using AgroMijo.API.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AgroMijo.API.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IMongoCollection<GameReport> _reports;

    public ReportsController(MongoDbService mongoDb)
    {
        _reports = mongoDb.Database
            .GetCollection<GameReport>("reports");
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport(
        GameReport report)
    {
        var existingReport = await _reports
            .Find(r => r.ReportId == report.ReportId)
            .FirstOrDefaultAsync();

        if (existingReport != null)
        {
            return Conflict(new
            {
                mensaje = "El reporte ya existe."
            });
        }

        await _reports.InsertOneAsync(report);

        return Ok(report);
    }

    [HttpGet("profile/{profileId}")]
    public async Task<IActionResult> GetReportsByProfile(
        string profileId)
    {
        var reports = await _reports
            .Find(r => r.ProfileId == profileId)
            .ToListAsync();

        return Ok(reports);
    }
}