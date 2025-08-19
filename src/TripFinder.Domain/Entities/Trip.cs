using RideApp.Domain;
using TripFinder.Domain.Entities;

public class Trip
{
    public int Id { get; set; } // PK
    public TripStatus Status { get; set; }
    public TripType Type { get; set; }

    public DateTime RequestDate { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime? DropoffDate { get; set; }

    public double PickupLat { get; set; }
    public double PickupLng { get; set; }
    public string PickupLocation { get; set; } = null!;
    public double DropoffLat { get; set; }
    public double DropoffLng { get; set; }
    public string DropoffLocation { get; set; } = null!;

    public int DurationMinutes { get; set; }
    public decimal DistanceKm { get; set; }
    public int CostKes { get; set; }

    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;
    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
}