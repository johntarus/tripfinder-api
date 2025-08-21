using System.ComponentModel.DataAnnotations;

namespace TripFinder.Core.Dtos;

public class SearchTripsRequestDto
{
    public string Q { get; set; } = string.Empty;
    
    public TripStatusFilter StatusFilter { get; set; } = TripStatusFilter.Completed;
    
    public DistanceRange? Distance { get; set; }
    public DurationRange? Duration { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;
    
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;
    
    public string SortBy { get; set; } = "RequestDate";
    public bool SortDescending { get; set; } = true;
}

public enum TripStatusFilter
{
    All,    
    Completed,  
    Cancelled,  
}

public enum DurationRange
{
    Quick,   
    Medium,  
    Long, 
    VeryLong
}

public enum DistanceRange
{
    Short,   
    Medium,  
    Long, 
    VeryLong 
}