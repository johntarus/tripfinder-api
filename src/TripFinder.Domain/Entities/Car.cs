namespace TripFinder.Domain.Entities;

public class Car
{
    public int Id { get; set; } 
    public string Number { get; set; } = null!; 
    public string Make { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public string? PictureUrl { get; set; }

    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;

    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}