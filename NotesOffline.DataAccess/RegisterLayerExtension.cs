using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotesOffline.DataAccess;

/// <summary>
/// This Extension is used to register Entity Framework database context and its entities
/// </summary>
public static class RegisterLayerExtension
{
    private const string DATABASE_NAME = "MasterDatabase";
    public static IServiceCollection RegisterDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseNpgsql(configuration.GetConnectionString(DATABASE_NAME))
                .UseSnakeCaseNamingConvention();
        });

        return services;
    }
}
