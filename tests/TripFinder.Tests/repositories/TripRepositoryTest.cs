using Microsoft.EntityFrameworkCore;
using RideApp.Domain;
using TripFinder.Domain.Entities;
using TripFinder.Infrastructure.Data;
using TripFinder.Infrastructure.Repositories;

namespace TripFinder.Tests.repositories
{
    public class TripRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;

        public TripRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private AppDbContext CreateContext() => new AppDbContext(_dbOptions);

        private void SeedTestData(AppDbContext db)
        {
            db.Trips.AddRange(new List<Trip>
            {
                new Trip
                {
                    Id = 1,
                    PickupLocation = "Nairobi",
                    DropoffLocation = "Kisumu",
                    Status = TripStatus.Completed,
                    DistanceKm = 12,
                    DurationMinutes = 20,
                    CostKes = 500,
                    RequestDate = new DateTime(2025, 1, 1)
                },
                new Trip
                {
                    Id = 2,
                    PickupLocation = "Mombasa",
                    DropoffLocation = "Nairobi",
                    Status = TripStatus.Canceled,
                    DistanceKm = 30,
                    DurationMinutes = 45,
                    CostKes = 1200,
                    RequestDate = new DateTime(2025, 1, 2)
                },
                new Trip
                {
                    Id = 3,
                    PickupLocation = "Nairobi",
                    DropoffLocation = "Naivasha",
                    Status = TripStatus.Completed,
                    DistanceKm = 60,
                    DurationMinutes = 90,
                    CostKes = 2000,
                    RequestDate = new DateTime(2025, 1, 3)
                }
            });

            db.SaveChanges();
        }

        [Fact]
        public async Task GetTripsAsync_ReturnsAllTrips()
        {
            using var db = CreateContext();
            SeedTestData(db);
            var repo = new TripRepository(db);

            var result = await repo.GetTripsAsync(CancellationToken.None);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetLatestTripsAsync_ReturnsOrderedTrips()
        {
            using var db = CreateContext();
            SeedTestData(db);
            var repo = new TripRepository(db);

            var result = await repo.GetLatestTripsAsync(2);

            Assert.Equal(2, result.Count());
            Assert.Equal(3, result.First().Id);
        }

        [Fact]
        public async Task GetTopDestinationsAsync_ReturnsGroupedDestinations()
        {
            using var db = CreateContext();
            SeedTestData(db);
            var repo = new TripRepository(db);

            var result = await repo.GetTopDestinationsAsync(2);

            Assert.NotEmpty(result);
            Assert.Contains(result, r => r.Destination == "Nairobi");
        }

    }
}
