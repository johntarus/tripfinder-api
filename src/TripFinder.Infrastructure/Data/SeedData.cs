using System.Text.Json;
using TripFinder.Domain.Entities;
using TripFinder.Domain.Enums;

namespace TripFinder.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Trips.Any())
            return; // Already seeded

        var httpClient = new HttpClient();
        var json = await httpClient.GetStringAsync("https://rapidtechinsights.github.io/hr-assignment/recent.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var root = JsonSerializer.Deserialize<TripsRootDto>(json, options);

        if (root?.Trips == null || !root.Trips.Any())
            return;

        var drivers = new Dictionary<int, Driver>();
        var cars = new Dictionary<string, Car>();
        var trips = new List<Trip>();

        foreach (var t in root.Trips)
        {
            // Handle driver (use JSON ID)
            if (!drivers.ContainsKey(t.DriverId))
            {
                drivers[t.DriverId] = new Driver
                {
                    Id = t.DriverId,
                    Name = t.DriverName,
                    Rating = t.DriverRating,
                    ProfilePicture = t.DriverPic
                };
            }

            // Handle car (database-generated ID)
            if (!cars.ContainsKey(t.CarNumber))
            {
                cars[t.CarNumber] = new Car
                {
                    Make = t.CarMake,
                    Model = t.CarModel,
                    Year = t.CarYear,
                    LicensePlate = t.CarNumber,
                    Photo = t.CarPic
                };
            }

            // Trip (use JSON ID)
            var trip = new Trip
            {
                Id = t.Id,
                RequestDate = t.RequestDate,
                PickupLocation = t.PickupLocation,
                DropoffLocation = t.DropoffLocation,
                PickupTime = t.PickupDate,
                DropoffTime = t.DropoffDate,
                Status = Enum.TryParse<TripStatus>(t.Status, true, out var status) ? status : TripStatus.Completed,
                Duration = t.Duration,
                Distance = (decimal)t.Distance,
                Cost = (decimal)t.Cost,
                Driver = drivers[t.DriverId],
                Car = cars[t.CarNumber]
            };

            trips.Add(trip);
        }

        // Save Drivers first (IDs from JSON)
        context.Drivers.AddRange(drivers.Values);
        await context.SaveChangesAsync();

        // Save Cars (IDs auto-generated)
        context.Cars.AddRange(cars.Values);
        await context.SaveChangesAsync();

        // Save Trips (IDs from JSON, with foreign keys)
        context.Trips.AddRange(trips);
        await context.SaveChangesAsync();
    }

    // Root object for JSON
    private class TripsRootDto
    {
        public List<TripJsonDto> Trips { get; set; } = new();
    }

    // DTO to map JSON trip
    private class TripJsonDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public double DropoffLat { get; set; }
        public double DropoffLng { get; set; }
        public string DropoffLocation { get; set; } = string.Empty;
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public int DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public decimal DriverRating { get; set; }
        public string DriverPic { get; set; } = string.Empty;
        public string CarMake { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public string CarNumber { get; set; } = string.Empty;
        public int CarYear { get; set; }
        public string CarPic { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string DurationUnit { get; set; } = string.Empty;
        public double Distance { get; set; }
        public string DistanceUnit { get; set; } = string.Empty;
        public double Cost { get; set; }
        public string CostUnit { get; set; } = string.Empty;
    }
}
