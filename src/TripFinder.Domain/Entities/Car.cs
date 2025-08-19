using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripFinder.Domain.Entities;

public class Car
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    
    public string Make { get; set; } = string.Empty;
    
    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }
    
    public string Color { get; set; } = string.Empty;

    public string Photo { get; set; } = string.Empty;

    public List<Trip> Trips { get; set; } = new();
}