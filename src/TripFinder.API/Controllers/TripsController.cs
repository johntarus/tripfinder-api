using Microsoft.AspNetCore.Mvc;
using TripFinder.Core.Dtos;
using TripFinder.Core.Interfaces.Services;

namespace TripFinder.API.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController(ITripService service) : ControllerBase
{
    [HttpGet("overtime")]
    [ProducesResponseType(typeof(IEnumerable<TripsOverTimeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTripsOverTime(CancellationToken ct)
    {
        var result = await service.GetTripsOverTimeAsync(ct);
        return Ok(result);
    }
    
    [HttpGet("latest")]
    [ProducesResponseType(typeof(IEnumerable<TripDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLatestTrips([FromQuery] int count = 5)
    {
        if (count <= 0) count = 5;

        var trips = await service.GetLatestTripsAsync(count);
        return Ok(trips);
    }
    
    [HttpGet("top-destinations")]
    [ProducesResponseType(typeof(List<DestinationCountDto>), 200)]
    public async Task<IActionResult> GetTopDestinations([FromQuery] int top = 3)
    {
        var result = await service.GetTopDestinationsAsync(top);
        return Ok(result);
    }
}