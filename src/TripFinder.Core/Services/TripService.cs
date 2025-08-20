using TripFinder.Core.Dtos;
using TripFinder.Core.Interfaces.Repositories;
using TripFinder.Core.Interfaces.Services;

namespace TripFinder.Core.Services;

public class TripService(ITripRepository repo) : ITripService
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
}