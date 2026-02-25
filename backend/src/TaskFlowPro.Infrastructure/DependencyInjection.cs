using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlowPro.Application.Common.Interfaces;
using TaskFlowPro.Infrastructure.Identity;
using TaskFlowPro.Infrastructure.Persistence;

namespace TaskFlowPro.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            )
        );

        // Register DbContext as the interface Application depends on
        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>()
        );

        // Identity services
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
