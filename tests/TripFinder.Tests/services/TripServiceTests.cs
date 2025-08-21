using Microsoft.Extensions.Logging;
using RideApp.Domain;
using TripFinder.Core.Dtos;
using TripFinder.Core.Interfaces.Repositories;
using TripFinder.Core.Services;
using TripFinder.Domain.Entities;

namespace TripFinder.Tests.services
{
    public class FakeTripRepository : ITripRepository
    {
        private readonly List<Trip> _trips;

        public FakeTripRepository()
        {
            _trips = new List<Trip>
            {
                new Trip
                {
                    Id = 1,
                    PickupLocation = "Nairobi",
                    DropoffLocation = "Kisumu",
                    Status = TripStatus.Completed,
                    DistanceKm = 10,
                    DurationMinutes = 20,
                    CostKes = 500,
                    RequestDate = new DateTime(2025,1,1)
                },
                new Trip
                {
                    Id = 2,
                    PickupLocation = "Mombasa",
                    DropoffLocation = "Nairobi",
                    Status = TripStatus.Canceled,
                    DistanceKm = 15,
                    DurationMinutes = 30,
                    CostKes = 1000,
                    RequestDate = new DateTime(2025,1,2)
                }
            };
        }

        public Task<List<Trip>> GetTripsAsync(CancellationToken ct)
        {
            return Task.FromResult(_trips);
        }

        public Task<IEnumerable<Trip>> GetLatestTripsAsync(int count)
        {
            var result = _trips.OrderByDescending(t => t.RequestDate).Take(count);
            return Task.FromResult(result);
        }

        public Task<List<DestinationCountDto>> GetTopDestinationsAsync(int top)
        {
            var result = _trips
                .GroupBy(t => t.DropoffLocation)
                .Select(g => new DestinationCountDto { Destination = g.Key, Count = g.Count() })
                .Take(top)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<PaginatedResponse<Trip>> SearchTripsAsync(SearchTripsRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _trips.AsQueryable();
            var items = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

            return Task.FromResult(new PaginatedResponse<Trip>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = query.Count()
            });
        }

        public Task<Trip?> GetTripByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_trips.FirstOrDefault(t => t.Id == id));
        }
    }

    public class TripServiceTests
    {
        private readonly TripService _service;

        public TripServiceTests()
        {
            var repo = new FakeTripRepository();
            var logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<TripService>();
            _service = new TripService(repo, logger);
        }

        [Fact]
        public async Task GetTripsOverTime_ReturnsMonthlyData()
        {
            var result = await _service.GetTripsOverTimeAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task GetLatestTrips_ReturnsTrips()
        {
            var result = await _service.GetLatestTripsAsync(1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetTopDestinations_ReturnsDestinations()
        {
            var result = await _service.GetTopDestinationsAsync(1);

            Assert.Single(result);
            Assert.NotNull(result[0].Destination);
        }

        [Fact]
        public async Task SearchTrips_ReturnsPaginatedResponse()
        {
            var request = new SearchTripsRequestDto { Page = 1, PageSize = 1 };

            var result = await _service.SearchTripsAsync(request, CancellationToken.None);

            Assert.Equal(1, result.Page);
            Assert.Equal(1, result.PageSize);
            Assert.True(result.TotalCount >= 1);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetTripById_ReturnsTrip_WhenExists()
        {
            var result = await _service.GetTripByIdAsync(1, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetTripById_ReturnsNull_WhenNotFound()
        {
            var result = await _service.GetTripByIdAsync(999, CancellationToken.None);

            Assert.Null(result);
        }
    }
}
