namespace TripFinder.Domain.Entities;

public class Driver
{
    public int Id { get; set; }  // PK, auto identity
    public int ExternalId { get; set; }  // JSON driver_id
    public string Name { get; set; } = null!;
    public double Rating { get; set; }
    public string? PictureUrl { get; set; }

    public ICollection<Car> Cars { get; set; } = new List<Car>();
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}