using System.ComponentModel.DataAnnotations;

namespace TripFinder.Core.Dtos;

public class SearchTripsRequestDto
{
    public string Q { get; set; } = string.Empty;
    public bool IncludeCancelled { get; set; } = false;
    public DistanceRange? Distance { get; set; }
    public DurationRange? Duration { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;
    
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;
    
    public string SortBy { get; set; } = "RequestDate";
    public bool SortDescending { get; set; } = true;
}

public enum DurationRange
{
    Quick,      // 0-15 min
    Medium,     // 15-30 min
    Long,       // 30-60 min
    VeryLong    // 60+ min
}

public enum DistanceRange
{
    Short,      // 0-5 km
    Medium,     // 5-15 km
    Long,       // 15-30 km
    VeryLong    // 30+ km
}