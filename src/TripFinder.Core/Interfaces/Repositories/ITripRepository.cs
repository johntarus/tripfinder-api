using TripFinder.Core.Dtos;
using TripFinder.Domain.Entities;

namespace TripFinder.Core.Interfaces.Repositories;

public interface ITripRepository
{
    Task<List<Trip>> GetTripsAsync(CancellationToken ct);
    Task<IEnumerable<Trip>> GetLatestTripsAsync(int count);
    Task<List<DestinationCountDto>> GetTopDestinationsAsync(int top = 3);
}