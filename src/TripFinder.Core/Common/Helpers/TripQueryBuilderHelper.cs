using RideApp.Domain;
using TripFinder.Core.Dtos;
using TripFinder.Domain.Entities;

namespace TripFinder.Core.Common.Helpers;

public static class TripQueryBuilderHelper
{
    public static IQueryable<Trip> ApplyFilters(IQueryable<Trip> query, SearchTripsRequestDto request)
    {
        query = request.StatusFilter switch
        {
            TripStatusFilter.Completed => query.Where(t => t.Status == TripStatus.Completed),
            TripStatusFilter.Cancelled => query.Where(t => t.Status == TripStatus.Canceled),
            TripStatusFilter.All or _ => query // Show all statuses (no filtering)
        };

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var searchTerm = request.Q.ToLower().Trim();
            query = query.Where(t =>
                (t.PickupLocation != null && t.PickupLocation.ToLower().Contains(searchTerm)) ||
                (t.DropoffLocation != null && t.DropoffLocation.ToLower().Contains(searchTerm)) ||
                (t.Type != null && t.Type.ToString().Contains(searchTerm)) ||
                (t.Driver != null && t.Driver.Name != null && t.Driver.Name.ToLower().Contains(searchTerm)) ||
                (t.Car != null && t.Car.Number != null && t.Car.Number.ToLower().Contains(searchTerm)) ||
                (t.Car != null && t.Car.Make != null && t.Car.Make.ToLower().Contains(searchTerm)) ||
                (t.Car != null && t.Car.Model != null && t.Car.Model.ToLower().Contains(searchTerm))
            );
        }

        if (request.Distance.HasValue)
        {
            query = request.Distance.Value switch
            {
                DistanceRange.Short => query.Where(t => t.DistanceKm <= 5),
                DistanceRange.Medium => query.Where(t => t.DistanceKm > 5 && t.DistanceKm <= 15),
                DistanceRange.Long => query.Where(t => t.DistanceKm > 15 && t.DistanceKm <= 30),
                DistanceRange.VeryLong => query.Where(t => t.DistanceKm > 30),
                _ => query
            };
        }

        if (request.Duration.HasValue)
        {
            query = request.Duration.Value switch
            {
                DurationRange.Quick => query.Where(t => t.DurationMinutes <= 15),
                DurationRange.Medium => query.Where(t => t.DurationMinutes > 15 && t.DurationMinutes <= 30),
                DurationRange.Long => query.Where(t => t.DurationMinutes > 30 && t.DurationMinutes <= 60),
                DurationRange.VeryLong => query.Where(t => t.DurationMinutes > 60),
                _ => query
            };
        }

        return query;
    }

    public static IQueryable<Trip> ApplySorting(IQueryable<Trip> query, SearchTripsRequestDto request)
    {
        var sortBy = request.SortBy?.ToLower() ?? "requestdate";
        var descending = request.SortDescending;

        return sortBy switch
        {
            "distance" => descending ? query.OrderByDescending(t => t.DistanceKm) : query.OrderBy(t => t.DistanceKm),
            "duration" => descending ? query.OrderByDescending(t => t.DurationMinutes) : query.OrderBy(t => t.DurationMinutes),
            "fare" => descending ? query.OrderByDescending(t => t.CostKes) : query.OrderBy(t => t.CostKes),
            "requestdate" or _ => descending ? query.OrderByDescending(t => t.RequestDate) : query.OrderBy(t => t.RequestDate)
        };
    }
}