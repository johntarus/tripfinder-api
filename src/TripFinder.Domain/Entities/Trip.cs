using TripFinder.Domain.Enums;

namespace TripFinder.Domain.Entities;

public class Trip
{
    public Guid Id { get; set; }
    public DateTime RequestDate { get; set; }
    public TripStatus Status { get; set; }
    public decimal Rating { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public string DropoffLocation { get; set; } = string.Empty;
    public DateTime PickupTime { get; set; }
    public DateTime DropoffTime { get; set; }
    public decimal Cost { get; set; }
    public decimal Distance { get; set; } 
    public int Duration { get; set; } 
    
    public Guid DriverId { get; set; }
    public Driver Driver { get; set; } = null!;
    
    public Guid CarId { get; set; }
    public Car Car { get; set; } = null!;
}