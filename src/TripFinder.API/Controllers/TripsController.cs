using Microsoft.AspNetCore.Mvc;
using TripFinder.Core.Dtos;
using TripFinder.Core.Interfaces.Services;

namespace TripFinder.API.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController(ITripService service, ILogger<TripsController> _logger) : ControllerBase
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
    
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<TripDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchTrips([FromQuery] SearchTripsRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Search trips request received: {@Request}", request);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid request parameters",
                    Errors = errors
                });
            }

            var result = await service.SearchTripsAsync(request, cancellationToken);

            return Ok(new ApiResponse<PaginatedResponse<TripDto>>
            {
                Success = true,
                Message = "Trips retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SearchTrips endpoint");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while processing your request"
            });
        }
    }
}