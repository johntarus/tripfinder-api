using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RideApp.Domain;
using TripFinder.Domain.Entities;

namespace TripFinder.Infrastructure.Data;

public static class DatabaseSeeder
{
    private static readonly HttpClient Http = new();

    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        var json = await Http.GetStringAsync("https://rapidtechinsights.github.io/hr-assignment/recent.json", ct);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var root = JsonSerializer.Deserialize<Root>(json, options)
                   ?? throw new InvalidOperationException("Failed to parse trips JSON.");

        // ---Save unique drivers ---
        var existingDrivers = await db.Drivers.ToListAsync(ct);
        var driverMap = existingDrivers.ToDictionary(d => d.ExternalId);

        foreach (var t in root.Trips.Select(x => new { x.driver_id, x.driver_name, x.driver_rating, x.driver_pic }).DistinctBy(d => d.driver_id))
        {
            if (!driverMap.ContainsKey(t.driver_id))
            {
                var driver = new Driver
                {
                    ExternalId = t.driver_id,
                    Name = t.driver_name,
                    Rating = t.driver_rating,
                    PictureUrl = t.driver_pic
                };
                db.Drivers.Add(driver);
                driverMap[t.driver_id] = driver;
            }
        }

        await db.SaveChangesAsync(ct);

        // --- Save unique cars ---
        var existingCars = await db.Cars.ToListAsync(ct);
        var carMap = existingCars.ToDictionary(c => c.Number, StringComparer.OrdinalIgnoreCase);

        foreach (var t in root.Trips.Select(x => new { x.car_number, x.car_make, x.car_model, x.car_year, x.car_pic, x.driver_id }).DistinctBy(c => c.car_number))
        {
            if (!carMap.ContainsKey(t.car_number))
            {
                var car = new Car
                {
                    Number = t.car_number,
                    Make = t.car_make,
                    Model = t.car_model,
                    Year = t.car_year,
                    PictureUrl = t.car_pic,
                    Driver = driverMap[t.driver_id]
                };
                db.Cars.Add(car);
                carMap[t.car_number] = car;
            }
        }

        await db.SaveChangesAsync(ct);

        // --- Save unique trips ---
        var existingTrips = await db.Trips
            .Select(tr => new { tr.RequestDate, tr.PickupLocation, tr.DropoffLocation })
            .ToListAsync(ct);

        var tripSet = new HashSet<(DateTime, string, string)>(
            existingTrips.Select(t => (t.RequestDate, t.PickupLocation, t.DropoffLocation))
        );

        foreach (var t in root.Trips)
        {
            var key = (ParseUtc(t.request_date), t.pickup_location, t.dropoff_location);

            if (!tripSet.Contains(key))
            {
                var trip = new Trip
                {
                    ExternalId = t.id, 
                    Status = ToStatus(t.status),
                    Type = ToType(t.type),
                    RequestDate = ParseUtc(t.request_date),
                    PickupDate = ParseUtc(t.pickup_date),
                    DropoffDate = string.IsNullOrWhiteSpace(t.dropoff_date) ? null : ParseUtc(t.dropoff_date),
                    PickupLat = t.pickup_lat,
                    PickupLng = t.pickup_lng,
                    PickupLocation = t.pickup_location,
                    DropoffLat = t.dropoff_lat,
                    DropoffLng = t.dropoff_lng,
                    DropoffLocation = t.dropoff_location,
                    DurationMinutes = t.duration,
                    DistanceKm = Math.Round((decimal)t.distance, 2),
                    CostKes = t.cost,
                    Driver = driverMap[t.driver_id],
                    Car = carMap[t.car_number]
                };

                db.Trips.Add(trip);
                tripSet.Add(key);
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static TripStatus ToStatus(string s) =>
        s.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase) ? TripStatus.Completed : TripStatus.Canceled;

    private static TripType ToType(string s) => s.ToUpperInvariant() switch
    {
        "BASIC" => TripType.Basic,
        "LADY" => TripType.Lady,
        "HAVAXL" => TripType.HavaXL,
        _ => TripType.Basic
    };

    private static DateTime ParseUtc(string s)
    {
        var dt = DateTime.ParseExact(s, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }

    private sealed class Root { public List<TripRow> Trips { get; set; } = new(); }

    private sealed class TripRow
    {
        public int driver_id { get; set; }
        public string driver_name { get; set; } = null!;
        public double driver_rating { get; set; }
        public string? driver_pic { get; set; }
        public string car_make { get; set; } = null!;
        public string car_model { get; set; } = null!;
        public string car_number { get; set; } = null!;
        public int car_year { get; set; }
        public string? car_pic { get; set; }
        public int id { get; set; }
        public string status { get; set; } = null!;
        public string request_date { get; set; } = null!;
        public double pickup_lat { get; set; }
        public double pickup_lng { get; set; }
        public string pickup_location { get; set; } = null!;
        public double dropoff_lat { get; set; }
        public double dropoff_lng { get; set; }
        public string dropoff_location { get; set; } = null!;
        public string pickup_date { get; set; } = null!;
        public string? dropoff_date { get; set; }
        public string type { get; set; } = null!;
        public int duration { get; set; }
        public double distance { get; set; }
        public int cost { get; set; }
    }
}
