using Microsoft.EntityFrameworkCore;
using TripFinder.Core.Common.Helpers;
using TripFinder.Core.Dtos;
using TripFinder.Core.Interfaces.Repositories;
using TripFinder.Domain.Entities;
using TripFinder.Infrastructure.Data;

namespace TripFinder.Infrastructure.Repositories;

public class TripRepository(AppDbContext dbContext) : ITripRepository
{
    /// <summary>
    /// Retrieves all trips from the database.
    /// </summary>
    public async Task<List<Trip>> GetTripsAsync(CancellationToken ct)
    {
        return await dbContext.Trips.AsNoTracking().ToListAsync(ct);
    }

    /// <summary>
    /// Retrieves the latest trips ordered by request date.
    /// </summary>
    public async Task<IEnumerable<Trip>> GetLatestTripsAsync(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive.", nameof(count));

        return await dbContext.Trips
            .OrderByDescending(t => t.RequestDate)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves the top destinations by trip count.
    /// </summary>
    public async Task<List<DestinationCountDto>> GetTopDestinationsAsync(int top = 3)
    {
        if (top <= 0)
            throw new ArgumentException("Top count must be positive.", nameof(top));

        return await dbContext.Trips
            .AsNoTracking()
            .GroupBy(t => t.PickupLocation)
            .Select(g => new DestinationCountDto
            {
                Destination = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToListAsync();
    }

    /// <summary>
    /// Builds a queryable for trips with included related data.
    /// </summary>
    public IQueryable<Trip> GetTripsQueryable()
    {
        return dbContext.Trips
            .Include(t => t.Driver)
            .Include(t => t.Car)
            .AsQueryable();
    }

    /// <summary>
    /// Searches trips based on the provided request parameters with pagination.
    /// </summary>
    public async Task<PaginatedResponse<Trip>> SearchTripsAsync(SearchTripsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Validate pagination parameters
        if (request.Page <= 0)
            throw new ArgumentException("Page number must be positive.", nameof(request.Page));
        if (request.PageSize <= 0)
            throw new ArgumentException("Page size must be positive.", nameof(request.PageSize));

        // Start with base query
        IQueryable<Trip> query = GetTripsQueryable();

        // Apply filters
        query = TripQueryBuilderHelper.ApplyFilters(query, request);

        // Apply sorting
        query = TripQueryBuilderHelper.ApplySorting(query, request);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<Trip>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Items = items
        };
    }
}