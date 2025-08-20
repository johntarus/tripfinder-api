using Microsoft.EntityFrameworkCore;
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
        return await dbContext.Trips
            .OrderByDescending(t => t.RequestDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<DestinationCountDto>> GetTopDestinationsAsync(int top = 3)
    {
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
}