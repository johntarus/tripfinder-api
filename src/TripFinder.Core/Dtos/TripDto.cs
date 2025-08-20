
namespace TripFinder.Core.Dtos;

public class TripDto
{
    public int Id { get; set; }
    public string Pickup { get; set; } = string.Empty;
    public string Dropoff { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string CarMake { get; set; } = string.Empty;
    public string CarModel { get; set; } = string.Empty;
    public string CarNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Distance { get; set; }
    public double Duration { get; set; }
    public decimal Fare { get; set; }
}