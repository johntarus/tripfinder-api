using TripFinder.Core.Dtos;
using TripFinder.Domain.Entities;

namespace TripFinder.Core.Common.Helpers;

public static class TimeSeriesHelper
{
    public static List<TripsOverTimeDto> GenerateMonthlyTimeSeries(List<Trip> trips)
    {
        var grouped = trips
            .GroupBy(t => new { t.RequestDate.Year, t.RequestDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToList();

        var minDate = trips.Any() ? trips.Min(t => t.RequestDate).Date : DateTime.UtcNow;
        var maxDate = trips.Any() ? trips.Max(t => t.RequestDate).Date : DateTime.UtcNow;

        var results = new List<TripsOverTimeDto>();
        var cursor = new DateTime(minDate.Year, minDate.Month, 1);

        while (cursor <= new DateTime(maxDate.Year, maxDate.Month, 1))
        {
            var match = grouped.FirstOrDefault(x => x.Year == cursor.Year && x.Month == cursor.Month);
            results.Add(new TripsOverTimeDto()
            {
                Date = new DateTime(cursor.Year, cursor.Month, 1),
                Value = match?.Count ?? 0
            });
            cursor = cursor.AddMonths(1);
        }

        return results;
    }
}