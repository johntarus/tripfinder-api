using Microsoft.Extensions.Logging;
using TripFinder.Core.Dtos;
using TripFinder.Core.Interfaces.Repositories;
using TripFinder.Core.Interfaces.Services;
using TripFinder.Domain.Entities;

namespace TripFinder.Core.Services;

public class TripService(ITripRepository repo, ILogger<TripService> _logger) : ITripService
{
    public async Task<List<TripsOverTimeDto>> GetTripsOverTimeAsync(CancellationToken ct)
    {
        var trips = await repo.GetTripsAsync(ct);

        var grouped = trips
            .GroupBy(t => new { t.RequestDate.Year, t.RequestDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToList();

        // Fill gaps
        var minDate = trips.Min(t => t.RequestDate).Date;
        var maxDate = trips.Max(t => t.RequestDate).Date;

        var results = new List<TripsOverTimeDto>();
        var cursor = new DateTime(minDate.Year, minDate.Month, 1);

        while (cursor <= new DateTime(maxDate.Year, maxDate.Month, 1))
        {
            var match = grouped.FirstOrDefault(x => x.Year == cursor.Year && x.Month == cursor.Month);
            results.Add(new TripsOverTimeDto
            {
                Date = new DateTime(cursor.Year, cursor.Month, 1),
                Value = match?.Count ?? 0
            });
            cursor = cursor.AddMonths(1);
        }

        return results;
    }

    public async Task<IEnumerable<TripDto>> GetLatestTripsAsync(int count = 5)
    {
        var trips = await repo.GetLatestTripsAsync(count);

        return trips.Select(t => new TripDto
        {
            Id = t.Id,
            Pickup = t.PickupLocation,
            Dropoff = t.DropoffLocation,
            DriverName = t.Driver != null ? t.Driver.Name : string.Empty,
            CarNumber = t.Car != null ? t.Car.Number : string.Empty,
            RequestDate = t.RequestDate,
            Status = t.Status.ToString()
        });
    }

    public async Task<List<DestinationCountDto>> GetTopDestinationsAsync(int top = 3)
    {
        return await repo.GetTopDestinationsAsync(top);

    }
    
    public async Task<PaginatedResponse<TripDto>> SearchTripsAsync(SearchTripsRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching trips with parameters: {@Request}", request);

            // Validate request
            if (request.Page <= 0) request.Page = 1;
            if (request.PageSize <= 0 || request.PageSize > 100) request.PageSize = 10;

            // Get data from repository
            var result = await repo.SearchTripsAsync(request, cancellationToken);

            // Map Trip entities to TripDto
            var tripDtos = result.Items.Select(MapToDto).ToList();

            return new PaginatedResponse<TripDto>
            {
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                Items = tripDtos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching trips");
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