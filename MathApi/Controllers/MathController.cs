using MathApi.Data;
using MathApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathApi.Controllers;

[ApiController]
[Route("api/mileage")]
public class MileageController(MathService svc, MileCalcDb db) : ControllerBase
{
    // Saves one row to the History table after every calculation.
    private void Log(string mode, string summary)
    {
        db.History.Add(new HistoryEntry { Mode = mode, Summary = summary });
        db.SaveChanges();
    }

    [HttpGet("mpg")]
    public ActionResult<MpgResult> Mpg([FromQuery] double miles, [FromQuery] double gallons)
    {
        if (miles <= 0 || gallons <= 0)
            return BadRequest(new { error = "miles and gallons must be positive" });
        var r = svc.Mpg(miles, gallons);
        Log("mpg", $"{miles} mi on {gallons} gal → {r.Mpg:F1} MPG");
        return r;
    }

    [HttpGet("trip-cost")]
    public ActionResult<TripCostResult> TripCost([FromQuery] double miles, [FromQuery] double mpg, [FromQuery] double ppg)
    {
        if (miles <= 0 || mpg <= 0 || ppg <= 0)
            return BadRequest(new { error = "all values must be positive" });
        var r = svc.TripCost(miles, mpg, ppg);
        Log("tripcost", $"{miles} mi @ {mpg} MPG @ ${ppg}/gal → ${r.TotalCost:F2}");
        return r;
    }

    [HttpGet("range")]
    public ActionResult<RangeResult> Range([FromQuery] double tank, [FromQuery] double mpg)
    {
        if (tank <= 0 || mpg <= 0)
            return BadRequest(new { error = "tank and mpg must be positive" });
        var r = svc.Range(tank, mpg);
        Log("range", $"{tank} gal @ {mpg} MPG → {r.RangeMiles:F0} mi range");
        return r;
    }

    [HttpGet("emissions")]
    public ActionResult<EmissionsResult> Emissions([FromQuery] double miles, [FromQuery] double mpg)
    {
        if (miles <= 0 || mpg <= 0)
            return BadRequest(new { error = "miles and mpg must be positive" });
        var r = svc.Emissions(miles, mpg);
        Log("emissions", $"{miles} mi @ {mpg} MPG → {r.Co2Kg:F1} kg CO₂");
        return r;
    }

    [HttpGet("savings")]
    public ActionResult<SavingsResult> Savings([FromQuery] double currentMpg, [FromQuery] double newMpg, [FromQuery] double annualMiles, [FromQuery] double ppg)
    {
        if (currentMpg <= 0 || newMpg <= 0 || annualMiles <= 0 || ppg <= 0)
            return BadRequest(new { error = "all values must be positive" });
        var r = svc.Savings(currentMpg, newMpg, annualMiles, ppg);
        Log("compare", $"{currentMpg} → {newMpg} MPG @ {annualMiles} mi/yr → ${r.AnnualSavings:F2} saved");
        return r;
    }

    // LINQ in action: filter, sort, and limit — EF Core turns this into SQL automatically.
    [HttpGet("history")]
    public ActionResult<IEnumerable<HistoryEntry>> GetHistory()
    {
        var entries = db.History
            .OrderByDescending(h => h.CreatedAt)   // newest first
            .Take(20)                               // max 20 rows
            .ToList();
        return Ok(entries);
    }

    [HttpDelete("history")]
    public IActionResult ClearHistory()
    {
        db.History.RemoveRange(db.History);
        db.SaveChanges();
        return NoContent();
    }

    [HttpGet("gas-price")]
    public async Task<ActionResult<GasPriceResult>> GasPrice([FromQuery] string? state, [FromServices] GasPriceService gasSvc)
        => Ok(await gasSvc.GetCurrentAsync(state));

    [HttpGet("version")]
    public IActionResult Version() => Ok(new { engine = svc.Engine });
}
