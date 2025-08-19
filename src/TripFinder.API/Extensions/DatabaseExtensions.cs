using Microsoft.EntityFrameworkCore;
using TripFinder.Infrastructure.Data;

namespace TripFinder.API.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }
}