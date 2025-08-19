namespace TripFinder.Domain.Entities;

public class Driver
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public string ProfilePicture { get; set; } = string.Empty;
    
    public List<Trip> Trips { get; set; } = new();
}