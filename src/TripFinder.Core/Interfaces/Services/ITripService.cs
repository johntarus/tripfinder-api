using TripFinder.Core.Dtos;

namespace TripFinder.Core.Interfaces.Services;

public interface ITripService
{
    Task<List<TripsOverTimeDto>> GetTripsOverTimeAsync(CancellationToken ct);
    Task<IEnumerable<TripDto>> GetLatestTripsAsync(int count = 5);
    Task<List<DestinationCountDto>> GetTopDestinationsAsync(int top = 3);
}