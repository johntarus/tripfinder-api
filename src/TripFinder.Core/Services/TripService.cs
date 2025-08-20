using Microsoft.Extensions.Logging;
using TripFinder.Core.Common.Helpers;
using TripFinder.Core.Dtos;
using TripFinder.Core.Interfaces.Repositories;
using TripFinder.Core.Interfaces.Services;
using TripFinder.Domain.Entities;

namespace TripFinder.Core.Services;

public class TripService(ITripRepository repo, ILogger<TripService> logger) : ITripService
{
    /// <summary>
    /// Retrieves the number of trips over time, grouped by month.
    /// </summary>
    public async Task<List<TripsOverTimeDto>> GetTripsOverTimeAsync(CancellationToken ct)
    {
        logger.LogInformation("Retrieving trips over time");
        var trips = await repo.GetTripsAsync(ct);
        return TimeSeriesHelper.GenerateMonthlyTimeSeries(trips);
    }

    /// <summary>
    /// Retrieves the latest trips as DTOs.
    /// </summary>
    public async Task<IEnumerable<TripDto>> GetLatestTripsAsync(int count = 5)
    {
        logger.LogInformation("Retrieving {Count} latest trips", count);
        count = count <= 0 ? 5 : count;

        var trips = await repo.GetLatestTripsAsync(count);
        return trips.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves the top destinations by trip count.
    /// </summary>
    public async Task<List<DestinationCountDto>> GetTopDestinationsAsync(int top = 3)
    {
        logger.LogInformation("Retrieving top {Top} destinations", top);
        top = top <= 0 ? 3 : top;
        return await repo.GetTopDestinationsAsync(top);
    }

    /// <summary>
    /// Searches trips based on the provided request parameters with pagination.
    /// </summary>
    public async Task<PaginatedResponse<TripDto>> SearchTripsAsync(SearchTripsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Searching trips with parameters: {@Request}", request);

            // Validate request
            if (request == null)
            {
                logger.LogWarning("Search request is null");
                throw new ArgumentNullException(nameof(request));
            }

            // Normalize pagination parameters
            request.Page = request.Page <= 0 ? 1 : request.Page;
            request.PageSize = request.PageSize <= 0 || request.PageSize > 100 ? 10 : request.PageSize;

            // Get data from repository
            var result = await repo.SearchTripsAsync(request, cancellationToken);

            // Map to DTOs
            var tripDtos = result.Items.Select(MapToDto).ToList();

            return new PaginatedResponse<TripDto>
            {
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                Items = tripDtos
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid search parameters: {@Request}", request);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching trips with parameters: {@Request}", request);
            throw new ApplicationException("An error occurred while searching trips", ex);
        }
    }

    private TripDto MapToDto(Trip trip)
    {
        return new TripDto
        {
            Id = trip.Id,
            Pickup = trip.PickupLocation,
            Dropoff = trip.DropoffLocation,
            Type = trip.Type.ToString() ?? string.Empty,
            DriverName = trip.Driver?.Name ?? string.Empty,
            CarMake = trip.Car?.Make ?? string.Empty,
            CarModel = trip.Car?.Model ?? string.Empty,
            CarNumber = trip.Car?.Number ?? string.Empty,
            RequestDate = trip.RequestDate,
            Status = trip.Status.ToString(),
            Distance = trip.DistanceKm,
            Duration = trip.DurationMinutes,
            Fare = trip.CostKes
        };
    }
}