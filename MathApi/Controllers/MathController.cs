using Microsoft.AspNetCore.Mvc;
using MathApi.NativeInterop;

namespace MathApi.Controllers;

[ApiController]
[Route("api/mileage")]
public class MileageController(MathService svc) : ControllerBase
{
    [HttpGet("mpg")]
    public IActionResult Mpg([FromQuery] double miles, [FromQuery] double gallons)
    {
        if (miles <= 0 || gallons <= 0)
            return BadRequest(new { error = "miles and gallons must be positive" });
        return Ok(svc.Mpg(miles, gallons));
    }

    [HttpGet("trip-cost")]
    public IActionResult TripCost([FromQuery] double miles, [FromQuery] double mpg, [FromQuery] double ppg)
    {
        if (miles <= 0 || mpg <= 0 || ppg <= 0)
            return BadRequest(new { error = "all values must be positive" });
        return Ok(svc.TripCost(miles, mpg, ppg));
    }

    [HttpGet("range")]
    public IActionResult Range([FromQuery] double tank, [FromQuery] double mpg)
    {
        if (tank <= 0 || mpg <= 0)
            return BadRequest(new { error = "tank and mpg must be positive" });
        return Ok(svc.Range(tank, mpg));
    }

    [HttpGet("emissions")]
    public IActionResult Emissions([FromQuery] double miles, [FromQuery] double mpg)
    {
        if (miles <= 0 || mpg <= 0)
            return BadRequest(new { error = "miles and mpg must be positive" });
        return Ok(svc.Emissions(miles, mpg));
    }

    [HttpGet("version")]
    public IActionResult Version() => Ok(new { engine = svc.Engine });
}
