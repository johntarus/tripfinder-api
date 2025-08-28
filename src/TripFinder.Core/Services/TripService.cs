using Microsoft.Extensions.Logging;
using TripFinder.Core.Common.Helpers;
using TripFinder.Core.Dtos;
using TripFinder.Core.Interfaces.Repositories;
using TripFinder.Core.Interfaces.Services;
using TripFinder.Domain.Entities;

namespace TripFinder.Core.Services;

public class TripService(ITripRepository repo, ILogger<TripService> _logger) : ITripService
{
    public async Task<List<TripsOverTimeDto>> GetTripsOverTimeAsync(CancellationToken ct)
    {
        _logger.LogInformation("Retrieving trips over time");
        var trips = await repo.GetTripsAsync(ct);
        return TimeSeriesHelper.GenerateMonthlyTimeSeries(trips);
    }

    public async Task<IEnumerable<TripDto>> GetLatestTripsAsync(int count = 5)
    {
        _logger.LogInformation("Retrieving {Count} latest trips", count);
        count = count <= 0 ? 5 : count;

        var trips = await repo.GetLatestTripsAsync(count);
        return trips.Select(MapToDto);
    }

    public async Task<List<DestinationCountDto>> GetTopDestinationsAsync(int top = 3)
    {
        _logger.LogInformation("Retrieving top {Top} destinations", top);
        top = top <= 0 ? 3 : top;
        return await repo.GetTopDestinationsAsync(top);
    }


    public async Task<PaginatedResponse<TripDto>> SearchTripsAsync(SearchTripsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching trips with parameters: {@Request}", request);

            if (request == null)
            {
                _logger.LogWarning("Search request is null");
                throw new ArgumentNullException(nameof(request));
            }

            request.Page = request.Page <= 0 ? 1 : request.Page;
            request.PageSize = request.PageSize <= 0 || request.PageSize > 100 ? 10 : request.PageSize;

            var result = await repo.SearchTripsAsync(request, cancellationToken);

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
            _logger.LogWarning(ex, "Invalid search parameters: {@Request}", request);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching trips with parameters: {@Request}", request);
            throw new ApplicationException("An error occurred while searching trips", ex);
        }
    }

    public async Task<TripDto?> GetTripByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching trip details for ID {TripId}", id);

            var trip = await repo.GetTripByIdAsync(id, cancellationToken);

            if (trip == null)
            {
                _logger.LogWarning("Trip with ID {TripId} not found", id);
                return null;
            }

            return MapToDto(trip);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving trip details for ID {TripId}", id);
            throw new ApplicationException($"An error occurred while retrieving trip {id}", ex);
        }
    }

    private TripDto MapToDto(Trip trip)
    {
        return new TripDto
        {
            Id = trip.Id,
            Pickup = trip.PickupLocation,
            PickupLat = trip.PickupLat,
            PickupLng = trip.PickupLng,
            Dropoff = trip.DropoffLocation,
            DropoffLat = trip.DropoffLat,
            DropoffLng = trip.DropoffLng,
            Type = trip.Type.ToString() ?? string.Empty,
            DriverName = trip.Driver?.Name ?? string.Empty,
            DriverRating = trip.Driver?.Rating ?? 0,
            DriverPicture = trip.Driver?.PictureUrl ?? string.Empty,
            PickupTime = trip.PickupDate,
            Year = trip.Car?.Year ?? 0,
            DropoffTime = trip.DropoffDate,
            CarMake = trip.Car?.Make ?? string.Empty,
            CarModel = trip.Car?.Model ?? string.Empty,
            CarNumber = trip.Car?.Number ?? string.Empty,
            CarPictureUrl = trip.Car?.PictureUrl ?? string.Empty,
            RequestDate = trip.RequestDate,
            Status = trip.Status.ToString(),
            Distance = trip.DistanceKm,
            Duration = trip.DurationMinutes,
            Fare = trip.CostKes
        };
    }
}