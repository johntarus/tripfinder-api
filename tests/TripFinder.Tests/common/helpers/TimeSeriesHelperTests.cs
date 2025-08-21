using TripFinder.Core.Common.Helpers;
using TripFinder.Domain.Entities;

namespace TripFinder.Tests.common.helpers;

public class TimeSeriesHelperTests
{
    [Fact]
    public void GenerateMonthlyTimeSeries_ReturnsCorrectCountsPerMonth()
    {
        var trips = new List<Trip>
        {
            new Trip { RequestDate = new DateTime(2025, 1, 5) },
            new Trip { RequestDate = new DateTime(2025, 1, 20) },
            new Trip { RequestDate = new DateTime(2025, 2, 10) },
            new Trip { RequestDate = new DateTime(2025, 4, 15) }
        };

        // Act
        var result = TimeSeriesHelper.GenerateMonthlyTimeSeries(trips);

        // Assert
        Assert.Equal(4, result.Count); 
        Assert.Equal(2, result[0].Value); 
        Assert.Equal(1, result[1].Value);
        Assert.Equal(0, result[2].Value); 
        Assert.Equal(1, result[3].Value);
        Assert.Equal(new DateTime(2025, 1, 1), result[0].Date);
        Assert.Equal(new DateTime(2025, 4, 1), result[3].Date);
    }

    [Fact]
    public void GenerateMonthlyTimeSeries_ReturnsEmptyList_WhenNoTrips()
    {
        // Arrange
        var trips = new List<Trip>();

        // Act
        var result = TimeSeriesHelper.GenerateMonthlyTimeSeries(trips);

        // Assert
        Assert.Single(result);
        Assert.Equal(0, result[0].Value);
        Assert.Equal(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), result[0].Date);
    }
}