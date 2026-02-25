using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TaskFlowPro.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR scans the assembly and registers all IRequestHandler implementations
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly)
        );

        // FluentValidation scans the assembly and registers all AbstractValidator implementations
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
