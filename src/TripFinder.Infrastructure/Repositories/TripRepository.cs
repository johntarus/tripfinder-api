using Microsoft.EntityFrameworkCore;
using TripFinder.Core.Common.Helpers;
using TripFinder.Core.Dtos;
using TripFinder.Core.Interfaces.Repositories;
using TripFinder.Domain.Entities;
using TripFinder.Infrastructure.Data;

namespace TripFinder.Infrastructure.Repositories;

public class TripRepository(AppDbContext dbContext) : ITripRepository
{
    public async Task<List<Trip>> GetTripsAsync(CancellationToken ct)
    {
        return await dbContext.Trips.AsNoTracking().ToListAsync(ct);
    }


    public async Task<IEnumerable<Trip>> GetLatestTripsAsync(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive.", nameof(count));

        return await dbContext.Trips
            .OrderByDescending(t => t.RequestDate)
            .Take(count)
            .ToListAsync();
    }
    
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
    
    public IQueryable<Trip> GetTripsQueryable()
    {
        return dbContext.Trips
            .Include(t => t.Driver)
            .Include(t => t.Car)
            .AsQueryable();
    }
    
    public async Task<PaginatedResponse<Trip>> SearchTripsAsync(SearchTripsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Page <= 0)
            throw new ArgumentException("Page number must be positive.", nameof(request.Page));
        if (request.PageSize <= 0)
            throw new ArgumentException("Page size must be positive.", nameof(request.PageSize));

        IQueryable<Trip> query = GetTripsQueryable();

        query = TripQueryBuilderHelper.ApplyFilters(query, request);

        query = TripQueryBuilderHelper.ApplySorting(query, request);

        var totalCount = await query.CountAsync(cancellationToken);

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

    public async Task<Trip?> GetTripByIdAsync(int id, CancellationToken ct)
    {
        return await dbContext.Trips
            .Include(t => t.Driver)
            .Include(t => t.Car)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }
}