using Microsoft.AspNetCore.Mvc;
using MathApi.Services;

namespace MathApi.Controllers;

[ApiController]
[Route("api/mileage")]
public class MileageController(MathService svc) : ControllerBase
{
    [HttpGet("mpg")]
    public ActionResult<MpgResult> Mpg([FromQuery] double miles, [FromQuery] double gallons)
    {
        if (miles <= 0 || gallons <= 0)
            return BadRequest(new { error = "miles and gallons must be positive" });
        return svc.Mpg(miles, gallons);
    }

    [HttpGet("trip-cost")]
    public ActionResult<TripCostResult> TripCost([FromQuery] double miles, [FromQuery] double mpg, [FromQuery] double ppg)
    {
        if (miles <= 0 || mpg <= 0 || ppg <= 0)
            return BadRequest(new { error = "all values must be positive" });
        return svc.TripCost(miles, mpg, ppg);
    }

    [HttpGet("range")]
    public ActionResult<RangeResult> Range([FromQuery] double tank, [FromQuery] double mpg)
    {
        if (tank <= 0 || mpg <= 0)
            return BadRequest(new { error = "tank and mpg must be positive" });
        return svc.Range(tank, mpg);
    }

    [HttpGet("emissions")]
    public ActionResult<EmissionsResult> Emissions([FromQuery] double miles, [FromQuery] double mpg)
    {
        if (miles <= 0 || mpg <= 0)
            return BadRequest(new { error = "miles and mpg must be positive" });
        return svc.Emissions(miles, mpg);
    }

    [HttpGet("version")]
    public IActionResult Version() => Ok(new { engine = svc.Engine });
}
